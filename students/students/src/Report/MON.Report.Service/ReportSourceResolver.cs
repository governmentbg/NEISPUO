using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Telerik.Reporting;
using Telerik.Reporting.Services;

namespace MON.Report.Service
{

    /// <summary>
    /// Този клас се използва от Telerik Reporting REST Service-а, т.е. от ReportController-а в WebApi проекта.
    /// Методът Resolve() се извиква 3 пъти при всяко генериране на справка / печатна бланка.
    /// Всеки път зарежда посочения .trdp файл, а на третия път зарежда данните за съответната справка и ги закача за ObjectDataSource-а на .trdp-то.
    /// Между трите извиквания автоматично се пази state. Тук е описано как става това:
    /// https://docs.telerik.com/reporting/using-telerik-reporting-in-applications-rest-service-cache-management-overview
    /// 
    /// Данните се връщат от най-различни service–и, но:
    /// 1. При всяко извикване на Resolve() е нужен само един от тези service-и - този за желаната справка.
    /// 2. Всяко генериране на справка извиква Resolve() 3 пъти, но service-ът е нужен само третия път.
    /// По тези две причини service-ите не са обявени като dependencies в конструктора,
    /// а се създават в последния момент в самия Resolve метод, при това класът се намира с reflection.
    /// </summary>
    public class ReportSourceResolver : IReportSourceResolver
    {
        private static readonly Assembly _thisAssembly = typeof(ReportSourceResolver).Assembly;
        private static readonly string _baseReportFilePath = Path.GetDirectoryName(_thisAssembly.Location);
        private static readonly string _baseReportServiceNamespace = typeof(ReportSourceResolver).Namespace;

        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public ReportSourceResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _logger = _serviceProvider.GetRequiredService<ILogger<ReportSourceResolver>>();
        }

        /// <summary>
        /// Зарежда посочения .trdp файл, зарежда от базата данните за съответната справка и закача данните за ObjectDataSource-а на .trdp-то.
        /// </summary>
        /// <param name="reportPath">
        /// ReportViewer контролата трябва да подаде релативен път до .trdp файла от вида "[Категория]\[Бланка]", например "Examination\AmbulatorySheet".
        /// </param>
        /// <param name="operationOrigin">
        /// Resolve методът се извиква 3 пъти при всяко генериране на справка. Тази enum стойност показва на коя стъпка сме.
        /// </param>
        /// <param name="parameters">
        /// Параметрите на справката, например id=123.
        /// На стъпка 1 параметрите се подават от ReportViewer-а. На стъпки 2 и 3 явно се извличат от кеширан Report обект.
        /// Имената на параметрите в .trdp файла трябва да съвпадат с подаваните от ReportViewer-а, иначе на стъпки 2 и 3 параметрите изчезват.
        /// </param>
        /// <returns>
        /// InstanceReportSource, който опакова parse–натия от .trdp файла report обект.
        /// </returns>
        public ReportSource Resolve(string reportPath, OperationOrigin operationOrigin, IDictionary<string, object> parameters)
        {
            Regex regex = new Regex(@"^(\d+).trdp");
            Match match = regex.Match(reportPath);
            if (match.Success)
            {
                // Подадено е име на отчет, което прилича на число, затова го изтегляме от базата
                int id = Convert.ToInt32(match.Groups[1].Value);

                DbDefinitionStorage dbDefinitionStorage = _serviceProvider.GetRequiredService<DbDefinitionStorage>();
                byte[] reportData = dbDefinitionStorage.GetDefinition(reportPath);

                Telerik.Reporting.Report report;
                using (MemoryStream memoryStream = new MemoryStream(reportData))
                {
                    var reportPackager = new ReportPackager();
                    report = reportPackager.Unpackage(memoryStream);
                }

                string reportBasicDocumentPath = dbDefinitionStorage.GetReportDefinition(reportPath);

                // Данните се зареждат от базата само ако това е последното, трето извикване.
                bool isLastCall = operationOrigin == OperationOrigin.GenerateReportDocument;
                if (isLastCall)
                {
                    AdjustParameterOptions(report, parameters);
                    try
                    {
                        // Ако тук гръмне грешка то на клиента ще се покаже нещо подобно "Unable to get report parameters: Report 'reportId' cannot be resolved.".
                        // Това е така независимо дали имаме или нямаме error handling.
                        // Трябва да внимаваме при зареждането на DataSource, защото cannot be resolved е много общо
                        string OSDependentReportPath = reportBasicDocumentPath.Replace('\\', Path.DirectorySeparatorChar);
                        Type reportServiceClass = FindReportServiceClass(OSDependentReportPath);
                        IReportService reportService = _serviceProvider.GetRequiredService(reportServiceClass) as IReportService;
                        report.DataSource = reportService?.LoadReport(parameters);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Грешка при зареждане на отчет {reportPath} {ex.Message}");
                        throw;
                    }
                }

                return new InstanceReportSource { ReportDocument = report };
            }
            else
            {
                string reportFilePath = GetReportFilePath(reportPath);

                // TODO: .trdp файлът се parse-ва на всяка от трите стъпки. Това е излишно, но не намерихме лесен начин да се избегне.
                // На първа и втора стъпка Report обектът се връща празен, а на трета стъпка - с данни.
                // В нормална ситуация се използва UriReportSource или TypeReportSource.
                // Тогава REST Service-ът създава Report обекта някъде извън Resolver-а, кешира го и го преизползва автоматично.
                // В случая обаче се използва InstanceReportSource и Report обектът се създава(parse-ва) изрично тук.
                // Пак се кешира някъде извън Resolver-а, но тук нямаме достъп до кеша, затова parse-ваме .trdp файла всеки път.
                // Кешът на REST Service-а е добре измислен - предвиден е за много сървъри, може да ползва база данни,
                // така че не е разумно да се прави второ кеширане, например с MemoryCache. Трябва да се "намери" готовият кеш.
                Telerik.Reporting.Report report;
                using (FileStream fileStream = File.OpenRead(reportFilePath))
                {
                    var reportPackager = new ReportPackager();
                    report = reportPackager.Unpackage(fileStream);

                }

                // Данните се зареждат от базата само ако това е последното, трето извикване.
                bool isLastCall = operationOrigin == OperationOrigin.GenerateReportDocument;

                if (isLastCall)
                {
                    AdjustParameterOptions(report, parameters);
                    // В този момент може да извършваме манипулации по отчета и променяме Visible на всички елементи, които започват с име background
                    //var backgroundItems = FindItems(report.Items.ToList(), "background");
                    //foreach (var item in backgroundItems)
                    //{
                    //    //item.Visible = false;
                    //}

                    try
                    {
                        // Ако тук гръмне грешка то на клиента ще се покаже нещо подобно "Unable to get report parameters: Report 'reportId' cannot be resolved.".
                        // Това е така независимо дали имаме или нямаме error handling.
                        // Трябва да внимаваме при зареждането на DataSource, защото cannot be resolved е много общо
                        string OSDependentReportPath = reportPath.Replace('\\', Path.DirectorySeparatorChar);
                        Type reportServiceClass = FindReportServiceClass(OSDependentReportPath);
                        IReportService reportService = _serviceProvider.GetRequiredService(reportServiceClass) as IReportService;
                        report.DataSource = reportService?.LoadReport(parameters);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Грешка при зареждане на отчет {reportPath} {ex.Message}");
                        throw;
                    }
                }

                return new InstanceReportSource { ReportDocument = report };
            }
        }

        private void AdjustParameterOptions(Telerik.Reporting.Report report, IDictionary<string, object> parameters)
        {
            var page1 = FindItems(report.Items.ToList(), "Page1", false).FirstOrDefault();
            var page2 = FindItems(report.Items.ToList(), "Page2", false).FirstOrDefault();
            var duplicateFlagTextBox = FindItems(report.Items.ToList(), "duplicateFlag", false).FirstOrDefault();
            // В случай, че искаме да имаме отстъпи като margins на целия report
            // report.PageSettings.Margins.Top = new Telerik.Reporting.Drawing.Unit($"{Convert.ToInt32(parameters["topMargin"])}mm");
            // report.PageSettings.Margins.Left = new Telerik.Reporting.Drawing.Unit($"{Convert.ToInt32(parameters["leftMargin"])}mm");

            if (page1 != null)
            {
                // Отстояние от ляво за страница 1
                if (parameters.ContainsKey("left1Margin"))
                {
                    page1.Style.Padding.Left = new Telerik.Reporting.Drawing.Unit($"{Convert.ToInt32(parameters["left1Margin"])}mm");
                }
                // Отстояние от горе за страница 1
                if (parameters.ContainsKey("top1Margin"))
                {
                    page1.Style.Padding.Top = new Telerik.Reporting.Drawing.Unit($"{Convert.ToInt32(parameters["top1Margin"])}mm");
                }
            }

            if (page2 != null)
            {
                // Отстояние от ляво за страница 2
                if (parameters.ContainsKey("left2Margin"))
                {
                    page2.Style.Padding.Left = new Telerik.Reporting.Drawing.Unit($"{Convert.ToInt32(parameters["left2Margin"])}mm");
                }
                // Отстояние от горе за страница 2
                if (parameters.ContainsKey("top2Margin"))
                {
                    page2.Style.Padding.Top = new Telerik.Reporting.Drawing.Unit($"{Convert.ToInt32(parameters["top2Margin"])}mm");
                }
            }

            if (duplicateFlagTextBox != null && parameters.ContainsKey("isDuplicate"))
            {
                bool isDuplicate = Convert.ToBoolean(parameters["isDuplicate"]);
                if (isDuplicate) { duplicateFlagTextBox.Visible = true; }
            }
        }

        /// <summary>
        /// Намира всички елементи, чиито имена започват с match
        /// </summary>
        /// <param name="reportItems"></param>
        /// <param name="match">текстов низ, по който да се търси в името</param>
        /// <param name="onlyLeaves">търси по име само в елементите от най-ниско ниво</param>/// 
        /// <returns></returns>
        public List<ReportItemBase> FindItems(List<ReportItemBase> reportItems, string match, bool onlyLeaves = true)
        {
            List<ReportItemBase> filteredReportItems = new List<ReportItemBase>();
            foreach (var item in reportItems)
            {
                if (item.Items != null && item.Items.Count > 0)
                {
                    if (!onlyLeaves)
                    {
                        if (item.Name.StartsWith(match))
                        {
                            filteredReportItems.Add(item);
                        }
                    }
                    filteredReportItems.AddRange(FindItems(item.Items.ToList(), match));
                }
                else
                {
                    if (item.Name.StartsWith(match))
                    {
                        filteredReportItems.Add(item);
                    }
                }
            }

            return filteredReportItems;
        }

        /// <summary>
        /// Търси клас с име от вида "...[Категория].[Вид]ReportService", например "...Student.DetailsReportService".
        /// </summary>
        /// <param name="reportPath">
        /// Релативен път до .trdp файла от вида "[Категория]\[Вид]", например "Student\Details".
        /// </param>
        private static Type FindReportServiceClass(string reportPath)
        {
            string className = $"{_baseReportServiceNamespace}.{reportPath.Replace(Path.DirectorySeparatorChar, '.').Replace(Path.AltDirectorySeparatorChar, '.')}ReportService";
            Type reportServiceType = _thisAssembly.GetType(className);

            if (reportServiceType == null)
            {
                throw new Exception($"Не съществува клас {className}.");
            }

            Type[] interfaces = reportServiceType.GetInterfaces();

            if (!reportServiceType.GetInterfaces().Any(x => x == typeof(IReportService)))
            {
                throw new Exception($"Клас {className} трябва да имплементира IReportService.");
            }

            return reportServiceType;
        }

        private string GetReportFilePath(string reportPath)
        {
            // При build .trdp файловете се копират в папката, в която е тази dll-ка (настройката е в .csproj файла).
            // Например, по време на разработка това е "bin\Debug\netcoreapp3.1\...".
            // Не е добро решение, но така този не-web проект не е нужно да знае в коя папка се хоства web приложението.
            string OSDependentReportPath = reportPath.Replace('\\', Path.DirectorySeparatorChar);
            string reportFilePath = Path.Combine(_baseReportFilePath, OSDependentReportPath + ".trdp");
            if (!File.Exists(reportFilePath))
            {
                reportFilePath = Path.Combine(_baseReportFilePath, OSDependentReportPath + ".trdp");
            }

            if (!File.Exists(reportFilePath))
            {
                throw new Exception($"Не съществува файл: {reportFilePath}.");
            }

            return reportFilePath;
        }

    }
}


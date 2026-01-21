using Telerik.Reporting.Cache.Interfaces;
using Telerik.Reporting.Services;

namespace MON.Report.Service
{

    /// <summary>
    /// Подава чрез DI настройките за ReportController-а в WebApi проекта.
    /// Не се използва, тъй като настройките се четат с друг клас от appsettings.json - виж коментарите в Startup.cs.
    /// </summary>
    public class ReportServiceConfiguration : Telerik.Reporting.Services.ReportServiceConfiguration
    {
        public ReportServiceConfiguration(IStorage storage, IReportSourceResolver reportSourceResolver)
        {
            Storage = storage;

            ReportSourceResolver = reportSourceResolver;

            // Възможно резултатът от справката да се кешира за определен брой минути. По подразбиране е изключено (0).
            // Ако заявката е със същите параметри, не се изпълнява третият Resolve(), т.е. не се зареждат данни от базата.
            //ReportSharingTimeout = 10;  // Минути.
        }
    }
}

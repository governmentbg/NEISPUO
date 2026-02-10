using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.IO;
using System.Runtime.InteropServices;
using Telerik.Reporting.Services;
using Telerik.WebReportDesigner.Services;

namespace MON.Report.Service
{
    public static class ServiceCollectionExtensions
    {
        public static void AddReportServiceLayer(this IServiceCollection services)
        {
            // ReportController изисква IReportServiceConfiguration, а той изисква IStorage и IReportSourceResolver.
            // IReportSourceResolver трябва да бъде scoped (а не singleton), защото използва много други scoped services.

            // Вариант 1: Интерфейсите се обявяват в DI. По-чисто е, но не може да се ползва appsettings.json.
            //services.TryAddSingleton<IStorage, FileStorage>();  // Не е ясно дали може да бъде singleton - пробваме.
            //services.AddScoped<IReportSourceResolver, ReportSourceResolver>();
            //services.AddScoped<IReportServiceConfiguration, ReportServiceConfiguration>();

            // Вариант 2: Настройките се четат от appsettings.json, но не може да се ползва DI.
            // Структурата е описана тук: https://docs.telerik.com/reporting/configuring-telerik-reporting-restreportservice
            // Вместо ReportServiceConfiguration трябва да се регистрира ConfigSectionReportServiceConfiguration.
            // Този клас обаче е sealed, затова няма как да се inject-ат IConfiguration и IReportSourceResolver.
            services.AddScoped<IReportServiceConfiguration>(serviceProvider =>
                new ConfigSectionReportServiceConfiguration
                {
                    ReportingEngineConfiguration = serviceProvider.GetRequiredService<IConfiguration>(),
                    // Resolver класът няма default constructor, затова не може да се укаже в appsettings.json, иначе става така:
                    //"reportResolver": {
                    //    "provider": "Hippocrates.Report.Service.ReportSourceResolver, Hippocrates.Report.Service"
                    //}
                    ReportSourceResolver = new ReportSourceResolver(serviceProvider),
                    Storage = serviceProvider.GetRequiredService<CacheMsSqlServerStorage>()
                });

            // Всички класове, които имплементират IScopedService се регистрират наведнъж.
            // В случая това са наследниците на клас ReportService.
            services.AddScopedAll<IScopedService>(typeof(ServiceCollectionExtensions).Assembly);

            // Конфигурирането продължава в Startup.cs на WebApi проекта, където се настройват web-ориентираните неща.
        }

        public static void AddReportDesignerLayer(this IServiceCollection services)
        {
            services.TryAddScoped<IReportDesignerServiceConfiguration>(sp => new ReportDesignerServiceConfiguration
            {
                DefinitionStorage = sp.GetRequiredService<DbDefinitionStorage>(),
                SettingsStorage = new FileSettingsStorage(RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                    ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telerik Reporting")
                    : "/app/Telerik Reporting"
                    )
            });
        }
    }
}

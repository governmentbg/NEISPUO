namespace SB.JobHost;

using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Prometheus;
using SB.Common;

public class JobHostModule : SBModule
{
    public const string ClassBookPrintHtmlJobRMSM = $"{nameof(ClassBookPrintHtmlJobRMSM)}";

    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JobHostOptions>()
            .Bind(configuration.GetSection("SB:JobHost"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.Configure<HostOptions>(opts =>
        {
            // TODO dotnet8 use ServicesStartConcurrently/ServicesStopConcurrently
            opts.ShutdownTimeout = configuration.GetValue<TimeSpan>("SB:JobHost:ShutdownTimeout");
        });

        if (configuration.GetValue<int>("SB:JobHost:PrintPdfJobOptions:JobInstances") is int htmlJobInstances
            && htmlJobInstances > 0)
        {
            for (int i = 0; i < htmlJobInstances; i++)
            {
                // TODO dotnet9 workaround for adding multiple instances
                // see https://github.com/dotnet/runtime/issues/38751#issuecomment-1158350910
                services.AddSingleton<IHostedService, PrintHtmlJob>();
            }
        }

        if (configuration.GetValue<int>("SB:JobHost:PrintPdfJobOptions:JobInstances") is int pdfJobInstances
            && pdfJobInstances > 0)
        {
            for (int i = 0; i < pdfJobInstances; i++)
            {
                // workaround for adding multiple instances
                // see https://github.com/dotnet/runtime/issues/38751#issuecomment-1158350910
                services.AddSingleton<IHostedService, PrintPdfJob>();
            }
        }

        if (configuration.GetValue<bool?>("SB:JobHost:MedicalNoticeJobOptions:IsEnabled") is true)
        {
            services.AddSingleton<IHostedService, MedicalNoticeJob>();
        }

        if (configuration.GetValue<int?>("SB:JobHost:EmailJobOptions:JobInstances") is int emailJobInstances && emailJobInstances > 0)
        {
            for (int i = 0; i < emailJobInstances; i++)
            {
                // workaround for adding multiple instances
                // see https://github.com/dotnet/runtime/issues/38751#issuecomment-1158350910
                services.AddSingleton<IHostedService, NotificationJob>();
            }
        }

        services.AddHealthChecks()
                .ForwardToPrometheus();
    }

    public override void ConfigureAutofacServices(ContainerBuilder builder, IConfiguration configuration)
    {
        builder
            .Register<RecyclableMemoryStreamManager>(c =>
            {
                var options = c.Resolve<IOptions<JobHostOptions>>();
                return new(
                    blockSize: 128 * 1024, // 128 KB
                    largeBufferMultiple: RecyclableMemoryStreamManager.DefaultLargeBufferMultiple, // irrelevant, as we dont use GetBuffer or initial cappacity
                    maximumBufferSize: RecyclableMemoryStreamManager.DefaultMaximumBufferSize, // irrelevant, as we dont use GetBuffer or initial cappacity
                    useExponentialLargeBuffer: false,
                    maximumSmallPoolFreeBytes: options.Value.PrintHtmlJobOptions.MemoryPoolSizeMB * 1024 * 1024,
                    maximumLargePoolFreeBytes: 0); // irrelevant, as we dont use GetBuffer or initial cappacity
            })
            .Named<RecyclableMemoryStreamManager>(ClassBookPrintHtmlJobRMSM)
            .SingleInstance();
    }
}

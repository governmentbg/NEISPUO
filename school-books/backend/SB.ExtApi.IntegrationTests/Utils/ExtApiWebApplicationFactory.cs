namespace SB.ExtApi.IntegrationTests;

using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SB.Data;
using SB.Domain;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

public class ExtApiWebApplicationFactory : WebApplicationFactory<Startup>
{
    protected override IHostBuilder? CreateHostBuilder()
    {
        var builder = base.CreateHostBuilder();
        if (builder is null)
        {
            return null;
        }

        // add a new named SerilogLoggerFactory that will hide the original audit logger(the last one wins)
        builder.ConfigureContainer<ContainerBuilder>(containerBuilder =>
        {
            containerBuilder
                .Register(c =>
                    new SerilogLoggerFactory(
                        new LoggerConfiguration()
                            // disable all audit logging,
                            // change next line if you want to print the logs to the console
                            .MinimumLevel.Fatal() // .MinimumLevel.Verbose()
                            .Enrich.FromLogContext()
                            .WriteTo.Console(
                                theme: AnsiConsoleTheme.Code,
                                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                            .CreateLogger(),
                        true))
                .Named<ILoggerFactory>("AuditLogLoggerFactoryName")
                .SingleInstance();
        });

        // add a new SerilogLoggerFactory that will hide the original logger(the last one wins)
        builder.UseSerilog((context, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                // logs are written only to the Debug Console (while debugging)
                // if you want to write the logs to the console, change the next line to .WriteTo.Console
                .WriteTo.Debug(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        });

        return builder;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.Configure<DataOptions>(opts =>
            {
                // use Poolling=False to avoid connection errors as the restore operation drops all existing connections
                opts.ConnectionString = DatabaseUtils.WebApplicationFactoryConnectionString;

                // use HiLoBlockSize of 1 to prevent errors with overlaping blocks due to DB restore before each test
                opts.HiLoBlockSize = 1;
            });
            services.Configure<DomainOptions>(opts =>
            {
                // disable caching as the ids will be repeating due to DB restore before each test
                opts.ShortCacheExpiration = TimeSpan.Zero;
                opts.MediumCacheExpiration = TimeSpan.Zero;
                opts.LongCacheExpiration = TimeSpan.FromMilliseconds(1);
            });
            services.Configure<HealthCheckPublisherOptions>(options =>
            {
                // disable all health checks
                options.Predicate = (_) => false;
            });
        });

        base.ConfigureWebHost(builder);
    }

    public ExtApiTestClient CreateExtApiClient(TestExtSystem testExtSystem)
    {
        HttpClient httpClient = this.CreateClient();

        // use this line to run the tests against
        // the API running in a docker container
        //HttpClient httpClient = new();

        if (Debugger.IsAttached)
        {
            // prevent HttpClient from timeouting when debugging
            httpClient.Timeout = Timeout.InfiniteTimeSpan;
        }

        return new ExtApiTestClient(httpClient, testExtSystem);
    }
}

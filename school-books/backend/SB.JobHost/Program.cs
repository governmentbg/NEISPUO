using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using SB.ApiAbstractions;
using SB.Common;
using SB.Data;
using SB.Domain;
using Serilog;
using Serilog.Debugging;

Console.WriteLine($"{DateTime.UtcNow:o} App starting");

SelfLog.Enable(Console.Out);
Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

try
{
    var modules =
        new SBModule[]
        {
            new DomainModule(),
            new DataModule(),
            new JobHostModule(),
        };

    var builder = WebApplication.CreateBuilder(args);

    builder.Host
        .ConfigureSerilog()
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .ConfigureContainer<ContainerBuilder>(
            (ctx, builder) => builder.RegisterModules(ctx.Configuration, modules))
        .ConfigureServices(
            (ctx, services) => services.RegisterModules(ctx.Configuration, modules));

    var app = builder.Build();

    // health check branch
    app.MapWhen(httpContext => httpContext.Connection.LocalPort == 9101, branchedApp => {
        branchedApp.UseHealthChecks("/healthz");
    });

    // metrics branch
    app.MapWhen(httpContext => httpContext.Connection.LocalPort == 9102, branchedApp => {
        branchedApp.UseMetricServer("/metrics");
    });

    // main branch
    app.MapGet("/api/version", ([FromServices] IWebHostEnvironment webHostEnvironment)
        => ControllerExtensions.GetVersion(webHostEnvironment));

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"{DateTime.UtcNow:o} {ex}");
    Console.WriteLine($"{DateTime.UtcNow:o} App terminated unexpectedly");

    return 1;
}
finally
{
    Log.CloseAndFlush();
}

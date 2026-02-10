namespace SB.ExtApi;

using System;
using System.ComponentModel;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using SB.ApiAbstractions;
using Serilog;
using Serilog.Debugging;

public static class Program
{
    public static int Main(string[] args)
    {
        Console.WriteLine($"{DateTime.UtcNow:o} App starting");

        // register custom type converters
        TypeDescriptor.AddAttributes(typeof(IPNetwork), new TypeConverterAttribute(typeof(IPNetworkTypeConverter)));

        SelfLog.Enable(Console.Out);
        Log.Logger = new LoggerConfiguration().CreateBootstrapLogger();

        try
        {
            CreateHostBuilder(args).Build().Run();

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
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureSerilog()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(
                (context, builder) => builder.ConfigureAuditSerilog(context))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

namespace SB.Cli;

using System;
using System.Diagnostics;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SB.Common;
using SB.Data;
using SB.Domain;

public static class AutofacSetup
{
    private class CliWebHostEnvironment : IWebHostEnvironment
    {
        public IFileProvider WebRootFileProvider { get; set; }
        public string WebRootPath { get; set; }
        public string ApplicationName { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public string EnvironmentName { get; set; }
    }

    public static IContainer ConfigureServices()
    {
        var container = CreateContainerBuilder().Build();
        return container;
    }

    public static ContainerBuilder CreateContainerBuilder()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            // Add env vars last as we'll use them to overwrite secret settings in production
            .AddEnvironmentVariables();
        var configuration = builder.Build();

        var services = new ServiceCollection();

        using var listener = new DiagnosticListener("Microsoft.AspNetCore");
        services.AddSingleton<DiagnosticListener>(listener);
        services.AddSingleton<DiagnosticSource>(listener);

        var hostingEnvironment = new CliWebHostEnvironment()
        {
            WebRootFileProvider = new NullFileProvider(),
            WebRootPath = null,
            ApplicationName = "SB.Cli",
            ContentRootFileProvider = new PhysicalFileProvider(AppContext.BaseDirectory),
            ContentRootPath = AppContext.BaseDirectory,
            EnvironmentName = "Development"
        };
        services.AddSingleton<IWebHostEnvironment>(hostingEnvironment);
        services.AddSingleton<IHostEnvironment>(hostingEnvironment);

        services.AddLogging(b =>
            b.AddConfiguration(configuration.GetSection("Logging"))
            .AddConsole());
        services.AddOptions();

        var containerBuilder = new ContainerBuilder();

        var modules = new SBModule[]
        {
            new DomainModule(),
            new DataModule(),
        };

        containerBuilder.RegisterModules(configuration, modules);

        services.RegisterModules(configuration, modules);

        containerBuilder.Populate(services);

        return containerBuilder;
    }
}

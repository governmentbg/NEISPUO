namespace SB.Blobs;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.SystemConsole.Themes;

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
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
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
            .UseSerilog((context, services, configuration) =>
            {
                var app = context.Configuration.GetValue<string>("SB:Blobs:App");
                var pod = context.Configuration.GetValue<string>("SB:Blobs:Pod");
                var version = Assembly.GetEntryAssembly()
                    ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    ?.InformationalVersion ?? string.Empty;

                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ApplyCustomOverrides(context.Configuration.GetSection("Serilog:MinimumLevel:OverrideList"))
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("App", app)
                    .Enrich.WithProperty("AppVersion", version)
                    .Enrich.WithProperty("Pod", pod)
                    .Filter.ByExcluding(logEvent => logEvent.Exception is OperationCanceledException)
                    .Filter.ByExcluding(logEvent => logEvent.Properties.ContainsKey("HealthCheckName"))
                    .Filter.ByExcluding(logEvent =>
                        logEvent.Exception is Microsoft.AspNetCore.Http.BadHttpRequestException &&
                        logEvent.Exception.Message == "Unexpected end of request content.");

                if (context.HostingEnvironment.IsDevelopment())
                {
                    configuration.WriteTo.Console(
                        theme: AnsiConsoleTheme.Code,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
                }
                else
                {
                    // write only Microsoft.Hosting.Lifetime to console
                    configuration.WriteTo.Logger(branchConfiguration =>
                    {
                        branchConfiguration
                        .Filter.ByIncludingOnly(Matching.FromSource("Microsoft.Hosting.Lifetime"))
                        .WriteTo.Console(new CompactJsonFormatter());
                    });
                }

                // SchoolBooksLog branch
                configuration.WriteTo.Logger(branchConfiguration =>
                {
                    branchConfiguration
                    .Filter.ByExcluding(Matching.FromSource("Microsoft.Hosting.Lifetime"))
                    .Enrich.With<ElapsedEnricher>()
                    .WriteTo.MSSqlServer(
                        connectionString: services.GetRequiredService<IOptions<DataOptions>>().Value.GetConnectionString(),
                        sinkOptions: new MSSqlServerSinkOptions
                        {
                            SchemaName = "logs",
                            TableName = "SchoolBooksLog",

                        },
                        logEventFormatter: new JsonLogEventFormatter(new[] { "ConnectionId" }),
                        columnOptions: new ColumnOptions()
                        {
                            Store = new List<StandardColumn>
                            {
                                StandardColumn.TimeStamp,
                                StandardColumn.Level,
                                StandardColumn.MessageTemplate,
                                StandardColumn.Exception,
                                StandardColumn.LogEvent
                            },
                            TimeStamp =
                            {
                                ColumnName = "DateUtc",
                                DataType = SqlDbType.DateTime2,
                                ConvertToUtc = true
                            },
                            LogEvent =
                            {
                                ExcludeAdditionalProperties = true,
                                ExcludeStandardColumns = true
                            },
                            AdditionalColumns = new List<SqlColumn>
                            {
                                new SqlColumn { ColumnName = "App", DataType = SqlDbType.NVarChar, DataLength = 100 },
                                new SqlColumn { ColumnName = "AppVersion", DataType = SqlDbType.NVarChar, DataLength = 50 },
                                new SqlColumn { ColumnName = "Pod", DataType = SqlDbType.NVarChar, DataLength = 100 },
                                new SqlColumn { ColumnName = "IpAddress", DataType = SqlDbType.NVarChar, DataLength = 50 },
                                new SqlColumn { ColumnName = "RequestMethod", DataType = SqlDbType.NVarChar, DataLength = 50 },
                                new SqlColumn { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = -1 },
                                new SqlColumn { ColumnName = "StatusCode", DataType = SqlDbType.NVarChar, DataLength = 50 },
                                new SqlColumn { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, DataLength = 250 },
                                new SqlColumn { ColumnName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = 50 },
                                new SqlColumn { ColumnName = "ActionName", DataType = SqlDbType.NVarChar, DataLength = 250 },
                                new SqlColumn { ColumnName = "ElapsedMs", PropertyName = "SB_Elapsed", DataType = SqlDbType.Int },
                                new SqlColumn { ColumnName = "SysUserId", DataType = SqlDbType.Int },
                                new SqlColumn { ColumnName = "SessionId", DataType = SqlDbType.NVarChar, DataLength = 50 },
                            }
                        }
                    );
                });
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

    public static LoggerConfiguration ApplyCustomOverrides(this LoggerConfiguration cfg, IConfigurationSection section)
        => section.GetChildren().Aggregate(cfg, (c, s) => c.MinimumLevel.Override(s.GetValue<string>("SourceContext"), s.GetValue<LogEventLevel>("Level")));
}

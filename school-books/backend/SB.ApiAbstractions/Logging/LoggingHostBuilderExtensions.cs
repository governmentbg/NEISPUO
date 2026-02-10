namespace SB.ApiAbstractions;

using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SB.Common;
using SB.Data;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Filters;
using Serilog.Formatting.Compact;
using Serilog.Sinks.MSSqlServer;
using Serilog.Sinks.SystemConsole.Themes;

public static class LoggingHostBuilderExtensions
{
    // change default message template to remove "Elapsed" property rendering
    public const string SerilogRequestLoggingMessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms";

    private static (string app, string pod, string version) GetAppPodVerion(HostBuilderContext context)
    {
        var app = context.Configuration.GetValue<string>("SB:ApiAbstractions:App") ?? "";
        var pod = context.Configuration.GetValue<string>("SB:ApiAbstractions:Pod") ?? "";
        var version = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? string.Empty;

        return (app, pod, version);
    }

    public static IHostBuilder ConfigureSerilog(this IHostBuilder builder)
    {
        return builder.UseSerilog((context, services, configuration) =>
        {
            var (app, pod, version) = GetAppPodVerion(context);

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
                    logEvent.Properties.TryGetValue("RequestPath", out var requestPath) &&
                    requestPath.ToString().Contains("getunreadconversations"));
            //TODO: Think of dynamic way to exclude logs based on request path

            // Console branch
            if (context.HostingEnvironment.IsDevelopment())
            {
                // write all to console
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
                .WriteTo.CustomMSSqlServer(
                    connectionString: services.GetRequiredService<IOptions<DataOptions>>().Value.GetConnectionString(),
                    sinkOptions: new MSSqlServerSinkOptions
                    {
                        SchemaName = "logs",
                        TableName = "SchoolBooksLog2",
                        EagerlyEmitFirstEvent = true,
                        BatchPeriod = TimeSpan.FromSeconds(10),
                        BatchPostingLimit = 1000,
                    },
                    queueLimit: 50000,
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
        });
    }

    public static void ConfigureAuditSerilog(this ContainerBuilder builder, HostBuilderContext context)
    {
        void UseNamedSerilogLoggerFactory(string loggerFactoryName, Action<HostBuilderContext, IServiceProvider, LoggerConfiguration> configureLogger)
        {
            builder
            .Register(c =>
            {
                var configuration = new LoggerConfiguration();
                var services = c.Resolve<IServiceProvider>();

                configureLogger(context, services, configuration);

                // the factory and the logger should be dispose by Autofac on App shutdown
                return new SerilogLoggerFactory(configuration.CreateLogger(), true);
            })
            .Named<ILoggerFactory>(loggerFactoryName)
            .SingleInstance();
        }

        UseNamedSerilogLoggerFactory("AuditLogLoggerFactoryName", (context, services, configuration) =>
        {
            var (app, pod, version) = GetAppPodVerion(context);

            configuration
            .Enrich.FromLogContext()
            .Enrich.WithProperty("App", app)
            .Enrich.WithProperty("AppVersion", version)
            .Enrich.WithProperty("Pod", pod)
            .MinimumLevel.Verbose() // log everything
            .WriteTo.CustomMSSqlServer(
                connectionString: services.GetRequiredService<IOptions<DataOptions>>().Value.GetConnectionString(),
                sinkOptions: new MSSqlServerSinkOptions
                {
                    SchemaName = "logs",
                    TableName = "Audit",
                    EagerlyEmitFirstEvent = true,
                    BatchPeriod = TimeSpan.FromSeconds(10),
                    BatchPostingLimit = 1000,
                },
                queueLimit: 50000,
                columnOptions: new ColumnOptions()
                {
                    Store = new List<StandardColumn> { StandardColumn.TimeStamp },
                    TimeStamp =
                    {
                        ColumnName = "DateUtc",
                        DataType = SqlDbType.DateTime2,
                        ConvertToUtc = true
                    },
                    AdditionalColumns = new List<SqlColumn>
                    {
                        new SqlColumn { ColumnName = "AuditCorrelationId", PropertyName = "RequestId", DataType = SqlDbType.NVarChar, DataLength = 50 },
                        new SqlColumn { ColumnName = "AuditModuleId", DataType = SqlDbType.Int },
                        new SqlColumn { ColumnName = "SysUserId", DataType = SqlDbType.Int },
                        new SqlColumn { ColumnName = "SysRoleId", DataType = SqlDbType.Int },
                        new SqlColumn { ColumnName = "Username", DataType = SqlDbType.NVarChar },
                        new SqlColumn { ColumnName = "FirstName", DataType = SqlDbType.NVarChar },
                        new SqlColumn { ColumnName = "MiddleName", DataType = SqlDbType.NVarChar },
                        new SqlColumn { ColumnName = "LastName", DataType = SqlDbType.NVarChar },
                        new SqlColumn { ColumnName = "LoginSessionId", DataType = SqlDbType.NVarChar, DataLength = 50 },
                        new SqlColumn { ColumnName = "RemoteIpAddress", DataType = SqlDbType.NVarChar, DataLength = 50 },
                        new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar },
                        new SqlColumn { ColumnName = "SchoolYear", DataType = SqlDbType.Int },
                        new SqlColumn { ColumnName = "InstId", DataType = SqlDbType.Int },
                        new SqlColumn { ColumnName = "PersonId", DataType = SqlDbType.Int },
                        new SqlColumn { ColumnName = "ObjectName", DataType = SqlDbType.NVarChar, DataLength = 50 },
                        new SqlColumn { ColumnName = "ObjectId", DataType = SqlDbType.Int },
                        new SqlColumn { ColumnName = "Action", PropertyName = "AuditAction", DataType = SqlDbType.NVarChar, DataLength = 50 },
                        new SqlColumn { ColumnName = "Data", PropertyName = "CommandJson", DataType = SqlDbType.NVarChar }
                    }
                }
            );
        });
    }
}

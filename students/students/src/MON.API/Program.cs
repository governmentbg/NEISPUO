using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MON.API.Logging.Sinks;
using MON.Services.Interfaces;
using MON.Shared.Enums;
using Prometheus;
using Serilog;
using Serilog.Exceptions;
using System;
using System.Data.SqlClient;
using System.Security.Authentication;

namespace MON.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    foreach (var source in config.Sources)
                    {
                        if (source is Microsoft.Extensions.Configuration.Json.JsonConfigurationSource jsonSource)
                        {
                            jsonSource.ReloadOnChange = false;
                        }
                    }
                })
                .UseSerilog((ctx, services, config) =>
                         {
                             // Подменя паролите в LogConnection и DefaultConnection с ST__Data__DbPass, за да може да се използват от Serilog
                             var dbPass = Environment.GetEnvironmentVariable("ST__Data__DbPass");
                             if (!string.IsNullOrWhiteSpace(dbPass))
                             {
                                 var sqlDefaultConnStringBuilder = new SqlConnectionStringBuilder(ctx.Configuration.GetConnectionString("DefaultConnection"));
                                 sqlDefaultConnStringBuilder.Password = dbPass;
                                 ctx.Configuration["ConnectionStrings:DefaultConnection"] = sqlDefaultConnStringBuilder.ToString();

                                 var LogConnection = ctx.Configuration.GetConnectionString("LogConnection");
                                 if (LogConnection != null)
                                 {
                                     var sqlLogConnStringBuilder = new SqlConnectionStringBuilder(LogConnection);
                                     sqlLogConnStringBuilder.Password = dbPass;
                                     ctx.Configuration["ConnectionStrings:LogConnection"] = sqlLogConnStringBuilder.ToString();
                                 }
                             }

                             config
                             .Enrich.FromLogContext()
                             .Enrich.WithThreadId()
                             .Enrich.WithExceptionDetails()
                             .Enrich.WithCorrelationId()
                             .Enrich.WithClientIp()
                             .Enrich.WithProperty("AuditModuleId", (int)AuditModuleEnum.Students)
                             .MinimumLevel.Debug()
                             .ReadFrom.Configuration(ctx.Configuration);
                         })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseIISIntegration();
                    webBuilder.ConfigureKestrel(kestrelOptions =>
                    {
                        // Без ограничение размера на заявките. По подразбиране е 28.6 MB
                        kestrelOptions.Limits.MaxRequestBodySize = null;
                        kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                        {
                            httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
                        });
                    });
                });
    }
}

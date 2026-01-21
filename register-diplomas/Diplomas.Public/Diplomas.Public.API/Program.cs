using Diplomas.Public.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using System;

namespace Diplomas.Public.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((ctx, config) =>
                {
                    // Подменя паролите в LogConnection и DefaultConnection с RD__Data__DbPass, за да може да се използват от Serilog
                    var dbPass = Environment.GetEnvironmentVariable("RD__Data__DbPass");
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
                    .Enrich.WithProperty("AuditModuleId", (int)AuditModuleEnum.Diplomas)
                    .MinimumLevel.Debug()
                    .ReadFrom.Configuration(ctx.Configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    .UseStartup<Startup>()
                    .UseIISIntegration();
                });
    }
}

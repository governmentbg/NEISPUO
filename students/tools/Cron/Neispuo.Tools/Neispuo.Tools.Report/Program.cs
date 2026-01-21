using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neispuo.Tools.DataAccess;
using Neispuo.Tools.Models.Configuration;
using Neispuo.Tools.Services.Implementations;
using Neispuo.Tools.Services.Interfaces;
using Neispuo.Tools.Shared.Enums;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

namespace Neispuo.Tools.Report
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = CreateHostBuilder(args).Build();

            var scope = builder.Services.CreateScope();
            var app = scope.ServiceProvider.GetRequiredService<Application>();

            await app.RunAsync(args);
            scope.Dispose();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddSingleton<IConfiguration>(Configuration);
            var sqlConnStringBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));
            var dbPass = Environment.GetEnvironmentVariable("ST__Data__DbPass");
            if (!string.IsNullOrWhiteSpace(dbPass))
            {
                sqlConnStringBuilder.Password = dbPass;
            }
            services.AddDbContext<NeispuoContext>(options =>
                options.UseSqlServer(
                    sqlConnStringBuilder.ConnectionString,
                    b => b.MigrationsAssembly(typeof(NeispuoContext).Assembly.FullName)).EnableSensitiveDataLogging());
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            services.AddScoped<Application>();
            services.AddScoped<IStatsService, StatsService>();
            services.AddScoped<TaskService, TaskService>();
            services.AddTransient<IEmailService, EmailService>();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
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
                //.Enrich.FromLogContext()
                //.Enrich.WithThreadId()
                //.Enrich.WithExceptionDetails()
                //.Enrich.WithCorrelationId()
                //.Enrich.WithClientIp()
                .Enrich.WithProperty("AuditModuleId", (int)AuditModuleEnum.Students)
                .MinimumLevel.Debug()
                .ReadFrom.Configuration(ctx.Configuration);
            })
            .ConfigureHostConfiguration(configuration =>
            {

            })
            .ConfigureServices((hostContext, services) =>
            {
                ConfigureServices(services, hostContext.Configuration);
            });
    }
}

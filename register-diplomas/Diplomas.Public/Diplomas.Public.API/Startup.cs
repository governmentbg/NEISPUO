using Diplomas.Public.API.Infrastructure.ErrorHandling;
using Diplomas.Public.DataAccess;
using Diplomas.Public.Models.Configuration;
using Diplomas.Public.Models.Settings;
using Diplomas.Public.Services;
using Diplomas.Public.Services.Implementations;
using Diplomas.Public.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Diplomas.Public.API.Identity;
using Microsoft.Data.SqlClient;
using Prometheus.SystemMetrics;
using Prometheus;
using Diplomas.Public.API.Extensions;

namespace Diplomas.Public.API
{
    public class Startup
    {
        public static readonly Encoding USAsciiStrict = Encoding.GetEncoding("us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AuthenticationConfig authConfig = Configuration.GetSection("Authentication").Get<AuthenticationConfig>();

            var sqlConnStringBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));
            var dbPass = Environment.GetEnvironmentVariable("RD__Data__DbPass");
            if (!string.IsNullOrWhiteSpace(dbPass))
            {
                sqlConnStringBuilder.Password = dbPass;
            }

            services.AddHealthChecks()
                .AddDbContextCheck<DiplomasContext>();
            services.AddDbContext<DiplomasContext>(options =>
                options.UseSqlServer(
                sqlConnStringBuilder.ConnectionString,
                b => b.MigrationsAssembly(typeof(DiplomasContext).Assembly.FullName)));

            services.Configure<CaptchaSettings>(Configuration.GetSection("Captcha"));
            services.Configure<BlobServiceConfig>(Configuration.GetSection("BlobService"));
            services.AddControllers();
            services.ConfigureCors(authConfig.AllowedCorsOrigins);
            ConfigureDependencyInjection(services);
            services.AddSystemMetrics();
            services.AddMetricServer(options => { options.Port = 9102; });
        }

        private static void AddSecurityHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
                // Strict policy below
                // context.Response.Headers.Add("Content-Security-Policy", "script-src 'self';style-src 'self' 'unsafe-inline';img-src 'self';font-src 'self';form-action 'self';frame-ancestors 'self';block-all-mixed-content");
                // Very permissive policy
                context.Response.Headers.Add("Content-Security-Policy", "default-src *  data: blob: filesystem: about: ws: wss: 'unsafe-inline' 'unsafe-eval' 'unsafe-dynamic'; script-src * 'unsafe-inline' 'unsafe-eval'; connect-src * 'unsafe-inline';  img-src * data: blob: 'unsafe-inline'; frame-src *; style-src * data: blob: 'unsafe-inline'; font-src * data: blob: 'unsafe-inline';");
                await next(context);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            bool isDevelopmentMode = env.IsDevelopment();
            if (isDevelopmentMode)
            {
                app.UseDeveloperExceptionPage();
            }

            AddSecurityHeaders(app);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors("CorsPolicy");
            app.UseHttpMetrics(options =>
            {
                // This will preserve only the first digit of the status code.
                // For example: 200, 201, 203 -> 2xx
                options.ReduceStatusCodeCardinality();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health", new HealthCheckOptions()
                {
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status200OK,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    }
                });
            });

            if (env.IsTestiis())
            {
                // Добавя fallback handler(endpoint) за всички заявки, които не са прихванати от контролер.
                // В production това са заявки за bundled статични файлове от папка wwwroot (или папката, обявена за root на компилираното SPA приложение).
                // Без този handler всички заявки, които не отговарят на контролер, връщат грешка 404.
                app.UseSpa(spa => { });
            }

        }

        private static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddOptions<CaptchaSettings>().BindConfiguration("Captcha");

            services.AddScoped<ApiExceptionFilter>();
            services.AddTransient<CaptchaVerificationService>();
            services.AddScoped<IDiplomaService, DiplomaService>();
            services.AddScoped<IUIErrorService, UIErrorService>();
            services.AddTransient<IAccessService, AccessService>();
        }
    }
}

using Helpdesk.API.ErrorHandling;
using Helpdesk.API.Extensions;
using Helpdesk.API.Identity;
using Helpdesk.DataAccess;
using Helpdesk.Models.Configuration;
using Helpdesk.Services.Implementations;
using Helpdesk.Services.Interfaces;
using Helpdesk.Shared.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Security.Claims;

namespace Helpdesk.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AuthenticationConfig authConfig = Configuration.GetSection("Authentication").Get<AuthenticationConfig>();
            services.AddApiAuthentication(authConfig);

            services.Configure<BlobServiceConfig>(Configuration.GetSection("BlobService"));
            services.Configure<AntiVirusConfig>(Configuration.GetSection("AntiVirus"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.AddHealthChecks()
                .AddDbContextCheck<HelpdeskContext>();

            var sqlConnStringBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));
            var dbPass = Environment.GetEnvironmentVariable("HD__Data__DbPass");
            if (!string.IsNullOrWhiteSpace(dbPass))
            {
                sqlConnStringBuilder.Password = dbPass;
            }

            services.AddDbContext<HelpdeskContext>(options =>
            {
                options.UseSqlServer(
                    sqlConnStringBuilder.ConnectionString,
                    b => b.MigrationsAssembly(typeof(HelpdeskContext).Assembly.FullName)).EnableSensitiveDataLogging();
                //options.ConfigureWarnings(w => w.Throw(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
                //options.ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            });

            services.AddControllers().AddNewtonsoftJson();
            services.ConfigureCors(authConfig.AllowedCorsOrigins);
            services.AddHttpContextAccessor();
            ConfigureDependencyInjection(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Helpdesk.API", Version = "v1" });
            });
        }

        private static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IIssueService, IssueService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IUIErrorService, UIErrorService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IAntiVirusService, AntiVirusService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAppConfigurationService, AppConfigurationService>();

            services.AddScoped<ApiExceptionFilter>();

            services.AddScoped<IUserInfo>(provider =>
            {
                var context = provider.GetService<IHttpContextAccessor>();

                var user = context.HttpContext.User;
                string clientIp = context.HttpContext.Connection.RemoteIpAddress.ToString();
                string jsonClaims = user?.FindFirstValue("selected_role");
                UserInfo userInfo = new UserInfo();
                if (jsonClaims != null)
                {
                    userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonClaims);
                }

                userInfo.ClientIp = clientIp;
                userInfo.UserAgent = context.HttpContext.Request.Headers[HeaderNames.UserAgent];
                userInfo.AccessToken = context.HttpContext.Request.Headers[HeaderNames.Authorization];
                return userInfo;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            bool isDevelopmentMode = env.IsDevelopment();
            if (isDevelopmentMode)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Helpdesk.API v1"));
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            if (env.IsTestiis())
            {
                // Добавя fallback handler(endpoint) за всички заявки, които не са прихванати от контролер.
                // В production това са заявки за bundled статични файлове от папка wwwroot (или папката, обявена за root на компилираното SPA приложение).
                // Без този handler всички заявки, които не отговарят на контролер, връщат грешка 404.
                app.UseSpa(spa => { });
            }
        }
    }
}

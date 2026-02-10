using Diplomas.Public.API.Extensions;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Domain;
using Hangfire;
using Kontrax.Regix.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using MON.API.ErrorHandling;
using MON.API.Identity;
using MON.API.Middleware;
using MON.API.Security;
using MON.ASPDataAccess;
using MON.BackgroundWorker;
using MON.DataAccess;
using MON.DataAccess.Interceptors;
using MON.Models.Configuration;
using MON.Models.Interfaces;
using MON.Report.Service;
using MON.Services;
using MON.Services.Hubs;
using MON.Services.Implementations;
using MON.Services.Implementations.DiplomaCode;
using MON.Services.Infrastructure.Validators;
using MON.Services.Infrastructure.Validators.StudentLod;
using MON.Services.Interfaces;
using MON.Services.Security;
using MON.Shared;
using MON.Shared.Interfaces;
using Newtonsoft.Json;
using Prometheus;
using Prometheus.SystemMetrics;
using Serilog;
using StackExchange.Redis;
using System;
using System.Buffers;
using System.Data.SqlClient;
using System.IO.Compression;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace MON.API
{
    public class Startup
    {
        public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

        /// <summary>
        /// Redis backplane for Hangfire
        /// </summary>
        public static ConnectionMultiplexer HangfireRedisConnection = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Изключване на hangfire
            //try
            //{
            //    HangfireConfig hangfireConfig = Configuration.GetSection("Hangfire").Get<HangfireConfig>();
            //    if (hangfireConfig != null && hangfireConfig.UseRedis)
            //    {
            //        HangfireRedisConnection = ConnectionMultiplexer.Connect(hangfireConfig.RedisConnectionString);
            //    }       
            //}
            //catch
            //{
            //    // Ignore
            //}
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AuthenticationConfig authConfig = Configuration.GetSection("Authentication").Get<AuthenticationConfig>();
            services.AddApiAuthentication(authConfig);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                // Динамичено компресиране с Brotli не е добър варианти при големи обеми от данни заради бързодействието
                //options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });

            services.Configure<BlobServiceConfig>(Configuration.GetSection("BlobService"));
            services.Configure<AntiVirusConfig>(Configuration.GetSection("AntiVirus"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<AuthenticationConfig>(Configuration.GetSection("Authentication"));
            services.Configure<SecuritySettings>(Configuration.GetSection("SecuritySettings"));
            services.Configure<CachingConfig>(Configuration.GetSection("Caching"));
            services.Configure<SignalRConfig>(Configuration.GetSection("SignalR"));
            services.AddHealthChecks()
                .AddDbContextCheck<MONContext>()
                .ForwardToPrometheus();

            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentName != null)
            {
                switch (environmentName.ToLower())
                {
                    case "prod":
                    case "test":
                    case "dev":
                        services.AddHsts(options =>
                        {
                            options.IncludeSubDomains = true;
                            options.MaxAge = TimeSpan.FromDays(365);
                        });
                        break;
                }
            }

            var sqlConnStringBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("DefaultConnection"));
            var dbPass = Environment.GetEnvironmentVariable("ST__Data__DbPass");
            if (!string.IsNullOrWhiteSpace(dbPass))
            {
                sqlConnStringBuilder.Password = dbPass;
            }

            services.AddDbContext<MONContext>(options =>
                options.UseSqlServer(
                    sqlConnStringBuilder.ConnectionString,
                    providerOptions => { 
                        providerOptions.MigrationsAssembly(typeof(MONContext).Assembly.FullName);
                    }
                ).EnableSensitiveDataLogging()
                .AddInterceptors(new TaggedQueryCommandInterceptor()));

            // Добавяне на DB контекст към базата за интеграция с АСП
            var sqlAspContextConnStringBuilder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("MonAspAbsenceConnection"));
            dbPass = Environment.GetEnvironmentVariable("ST__ASP__DbPass");
            if (!string.IsNullOrWhiteSpace(dbPass))
            {
                sqlAspContextConnStringBuilder.Password = dbPass;
            }

            services.AddDbContext<MONASPContext>(options =>
                options.UseSqlServer(
                    sqlAspContextConnStringBuilder.ConnectionString,
                    b => b.MigrationsAssembly(typeof(MONASPContext).Assembly.FullName)).EnableSensitiveDataLogging());

            services.Configure<RegixConfig>(Configuration.GetSection("Regix"));
            services.Configure<ChangeTrackerLogConfig>(Configuration.GetSection("ChangeTrackerLog"));
            services.Configure<UserManagementServiceConfig>(Configuration.GetSection("UserManagementService"));
            services.AddControllers(o =>
            {
                o.Conventions.Add(new ActionHidingConvention());
            }).AddNewtonsoftJson();

            services.AddOptions<MvcOptions>()
                  .PostConfigure<IOptions<JsonOptions>, IOptions<MvcNewtonsoftJsonOptions>, ArrayPool<char>, ObjectPoolProvider, ILoggerFactory>(
                      (mvcOptions, jsonOpts, newtonJsonOpts, charPool, objectPoolProvider, loggerFactory) =>
                      {
                          var formatter = mvcOptions.InputFormatters.OfType<NewtonsoftJsonInputFormatter>().First(i => i.SupportedMediaTypes.Contains("application/json"));
                          formatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/csp-report"));
                          mvcOptions.InputFormatters.RemoveType<NewtonsoftJsonInputFormatter>();
                          mvcOptions.InputFormatters.Add(formatter);
                      });

            services.ConfigureCors(authConfig.AllowedCorsOrigins);
            services.AddHttpContextAccessor();

            SignalRConfig signalRConfig = Configuration.GetSection("SignalR").Get<SignalRConfig>();
            if (signalRConfig != null && signalRConfig.UseRedis)
            {
                services.AddSignalR().AddStackExchangeRedis(signalRConfig.RedisConnectionString, options =>
                {
                    options.Configuration.ChannelPrefix = signalRConfig.ChannelPrefix;
                });
            }
            else
            {
                services.AddSignalR();
            }

            CachingConfig cachingConfig = Configuration.GetSection("Caching").Get<CachingConfig>();
            if (cachingConfig != null && cachingConfig.UseRedis)
            {
                ConnectionMultiplexer.SetFeatureFlag("preventthreadtheft", true);
                services.AddStackExchangeRedisCache(options =>
                {
                    options.ConfigurationOptions = new ConfigurationOptions
                    {
                        Password = cachingConfig.RedisPassword.IsNullOrWhiteSpace() ? null : cachingConfig.RedisPassword,
                        EndPoints = { cachingConfig.RedisConnectionString },
                    };
                });
            }
            else
            {
                services.AddDistributedMemoryCache();
            }

            ConfigureDependencyInjection(services);

            services.AddSwaggerGen(c =>
                {
                    // Използват се пълните имена на класовете, за да се избегнат конфликти
                    c.CustomSchemaIds(i => i.FullName);
                }
            );

            services.AddReportServiceLayer();
            services.AddReportDesignerLayer();
            services.AddMemoryCache();
            services.AddHttpClient();

            // Този проект съдържа само Web API контролери без UI, затова AddControllers() е достатъчно.
            // Ако се добавят MVC контролери с Razor views, вместо AddControllers() трябва да се изпълни AddControllersWithViews().
            IMvcBuilder mvcBuilder = services.AddControllers();

            ConfigureTelerikReporting(services, mvcBuilder);
            services.AddSystemMetrics();

            if (HangfireRedisConnection != null)
            {
                // https://docs.hangfire.io/en/latest/getting-started/aspnet-core-applications.html
                // Add Hangfire services.
                services.AddHangfire(configuration =>
                {
                    configuration.UseRedisStorage(HangfireRedisConnection);
                });

                // Add the processing server as IHostedService
                services.AddHangfireServer();
            }
        }

        /// <summary>
        /// Web-ориентирани настройки на reporting-а на Telerik, които не могат да се изнесат в AddReportServiceLayer().
        /// </summary>
        private void ConfigureTelerikReporting(IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            // Asp.net core 3 по подразбиране забранява синхронните операции, за да не се изчерпват нишките за обработка на заявки,
            // но зареждането на картинки и други "ресурси" в справките на Telerik изисква такива синхронни операции.
            // Амбулаторният лист например зарежда картника за пунктирана линия, което гърми без тази настройка.
            services.Configure<IISServerOptions>(options => { options.AllowSynchronousIO = true; });

            // Asp.net core 3.1 по подразбиране ползва негов си сериализатор.
            // Reporting контролерът на Telerik обаче изисква точно сериализатора на Newtonsoft.
            // TODO: Ако е възможно, за останалите контролери да се използва вграденият.
            mvcBuilder.AddNewtonsoftJson();
        }

        private static void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddScoped(typeof(DbServiceDependencies<>));
            services.AddScoped(typeof(MovementDocumentServiceDependencies<>));

            services.AddScoped<INeispuoAuthenticationService, NeispuoAuthenticationService>();
            services.AddScoped<INeispuoAuthorizationService, NeispuoAuthorizationService>();

            services.AddScoped<AdmissionDocumentValidator>();
            services.AddScoped<RelocationDocumentValidator>();
            services.AddScoped<DischargeDocumentValidator>();
            services.AddScoped<StudentClassValidationContext>();
            services.AddScoped<LodFinalizationValidator>();
            services.AddScoped<LodEvaluationsValidator>();
            services.AddScoped<AspValidator>();
            services.AddScoped<AbsencesImportValidator>();

            services.AddTransient<IStudentService, StudentService>();
            services.AddTransient<IFinanceService, FinanceService>();
            services.AddTransient<IStudentLODService, StudentLODService>();
            services.AddTransient<ILookupService, LookupsService>();
            services.AddTransient<IHistoryService, HistoryService>();
            services.AddTransient<IDocumentService, DocumentService>();
            services.AddTransient<IStudentClassService, StudentClassService>();
            services.AddTransient<IBlobService, BlobService>();
            services.AddTransient<IAdministrationService, AdministrationService>();
            services.AddTransient<IOtherInstitutionService, OtherInstitutionService>();
            services.AddTransient<IStudentScholarshipService, StudentScholarshipService>();
            services.AddTransient<IStudentSOPService, StudentSOPService>();
            services.AddTransient<IResourceSupportService, ResourceSupportService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IEnvironmentCharacteristicsService, EnvironmentCharacteristicsService>();
            services.AddTransient<IInstitutionService, InstitutionService>();
            services.AddTransient<IAbsenceService, AbsenceService>();
            services.AddTransient<INomenclatureService, NomenclatureService>();
            services.AddTransient<IDiplomaService, DiplomaService>();
            services.AddTransient<IExternalEvaluationService, ExternalEvaluationService>();
            services.AddTransient<IPreSchoolEvaluationService, PreSchoolEvaluationService>();
            services.AddTransient<IBasicDocumentPartsService, BasicDocumentPartsService>();
            services.AddTransient<IDashboardService, DashboardService>();
            services.AddTransient<IStudentAwardService, StudentAwardService>();
            services.AddTransient<IDischargeDocumentService, DischargeDocumentService>();
            services.AddTransient<IStudentSanctionService, StudentSanctionService>();
            services.AddTransient<IAdmissionDocumentService, AdmissionDocumentService>();
            services.AddTransient<IDynamicFormService, DynamicFormService>();
            services.AddTransient<IRelocationDocumentService, RelocationDocumentService>();
            services.AddTransient<IBarcodeYearService, BacodeYearService>();
            services.AddTransient<ISchoolBooksService, SchoolBooksService>();
            services.AddTransient<ISchoolTypeLodAccessService, SchoolTypeLodAccessService>();
            services.AddTransient<IBasicDocumentService, BasicDocumentService>();
            services.AddTransient<IBarcodeService, BarcodeService>();
            services.AddTransient<IOtherDocumentService, OtherDocumentService>();
            services.AddTransient<IRecognitionService, RecognitionService>();
            services.AddTransient<IEqualizationService, EqualizationService>();
            services.AddTransient<IStudentSelfGovernmentService, StudentSelfGovernmentService>();
            services.AddTransient<IStudentInternationalMobilityService, StudentInternationalMobilityService>();
            services.AddTransient<IClassGroupService, ClassGroupService>();
            services.AddTransient<IPrintTemplateService, PrintTemplateService>();
            services.AddTransient<IStudentPersonalDevelopmentSupportService, StudentPersonalDevelopmentSupportService>();
            services.AddTransient<IUserManagementService, UserManagementService>();
            services.AddTransient<IAdmissionPermissionRequestService, AdmissionPermissionRequestService>();
            services.AddTransient<IAspService, AspService>();
            services.AddTransient<ILodFinalizationService, LodFinalizationService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddTransient<IUIErrorService, UIErrorService>();
            services.AddTransient<IAntiVirusService, AntiVirusService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IAbsenceCampaignService, AbsenceCampaignService>();
            services.AddTransient<INoteService, NoteService>();
            services.AddTransient<IRefugeeService, RefugeeService>();
            services.AddTransient<ICertificateService, CertificateService>();
            services.AddTransient<IDiplomaImportValidationExclusionService, DiplomaImportValidationExclusionService>();
            services.AddTransient<IDiplomaTemplateService, DiplomaTemplateService>();
            services.AddTransient<IHealthInsuranceService, HealthInsuranceService>();
            services.AddTransient<IDiplomaCreateRequestService, DiplomaCreateRequestService>();
            services.AddTransient<IWordTemplateService, WordTemplateService>();
            services.AddTransient<CurriculumService>();
            services.AddTransient<IBasicDocumentMarginService, BasicDocumentMarginService>();
            services.AddTransient<ILeadTeacherService, LeadTeacherService>();
            services.AddTransient<ILodAssessmentService, LodAssessmentService>();
            services.AddTransient<IReassessmentService, ReassessmentService>();
            services.AddScoped<IOresService, OresService>();
            services.AddScoped<ICommonPersonalDevelopmentSupportService, CommonPersonalDevelopmentSupportService>();
            services.AddScoped<IAdditionalPersonalDevelopmentSupportService, AdditionalPersonalDevelopmentSupportService>();
            services.AddScoped<IEarlyAssessmentService, EarlyAssessmentService>();
            services.AddScoped<ILodAssessmentTemplateService, LodAssessmentTemplateService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<DocManagementCampaignService>();
            services.AddScoped<DocManagementApplicationService>();
            services.AddScoped<DocManagementExchangeService>();
            services.AddScoped<DocManagementAdditionalCampaignService>();

            services.AddScoped<ISignalRNotificationService, SignalRNotificationService>();

            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<EduStateCacheService>();

            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            // Всички класове, които имплементират IScopedService се регистрират наведнъж.
            // В случая това са наследниците на клас CodeService.
            services.AddScopedAll<Services.Interfaces.IScopedService>(typeof(CodeService).Assembly);
            services.AddScoped<IAppConfigurationService, AppConfigurationService>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<ApiExceptionFilter>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserInfo>(provider =>
            {
                var context = provider.GetService<IHttpContextAccessor>();

                var user = context.HttpContext?.User;
                string clientIp = context.HttpContext?.Connection.RemoteIpAddress.ToString();
                string jsonClaims = user?.FindFirstValue("selected_role");
                UserInfo userInfo = new UserInfo();
                if (jsonClaims != null)
                {
                    userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonClaims);
                }
                userInfo.LoginSessionId = user?.FindFirstValue("sessionId");

                //В момента данните за имперсонатора не идват в токена и съответно не може да ги прочетем
                string impersonator = user?.FindFirstValue("impersonator");
                string impersonatorSysUserId = user?.FindFirstValue("impersonatorSysUserID");
                userInfo.Impersonator = impersonator;
                if (Int32.TryParse(impersonatorSysUserId, out var intImpersonatorSysUserId))
                {
                    userInfo.ImpersonatorSysUserID = intImpersonatorSysUserId;
                }

                userInfo.ClientIp = clientIp;
                userInfo.UserAgent = context.HttpContext?.Request.Headers[HeaderNames.UserAgent];
                userInfo.AccessToken = context.HttpContext?.Request.Headers[HeaderNames.Authorization];

                return userInfo;
            });
        }

        private void AddSecurityHeaders(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                //context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("Permissions-Policy", "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");
                // Strict policy below
                // context.Response.Headers.Add("Content-Security-Policy", "script-src 'self';style-src 'self' 'unsafe-inline';img-src 'self';font-src 'self';form-action 'self';frame-ancestors 'self';block-all-mixed-content; report-uri /api/error/cspreport");
                // Very permissive policy
                context.Response.Headers.Add("Content-Security-Policy", "default-src *  data: blob: filesystem: about: ws: wss: 'unsafe-inline' 'unsafe-eval' 'unsafe-dynamic'; script-src * 'unsafe-inline' 'unsafe-eval';  connect-src * 'unsafe-inline';   img-src * data: blob: 'unsafe-inline';                frame-src * blob:;                style-src * data: blob: 'unsafe-inline'; font-src * data: blob: 'unsafe-inline'; report-uri /api/error/cspreport");
                await next();
            });
        }

        public void PushAdditionalSeriLogProperties(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var user = httpContext?.User;
            string jsonClaims = user?.FindFirstValue("selected_role");
            if (jsonClaims != null)
            {
                var userInfo = JsonConvert.DeserializeObject<UserInfo>(jsonClaims);
                diagnosticContext.Set("SysUserId", userInfo?.SysUserID);
                diagnosticContext.Set("SysRoleId", userInfo?.SysRoleID);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            BlobServiceConfig blobServiceConfig = Configuration.GetSection("BlobService").Get<BlobServiceConfig>();

            bool isDevelopmentMode = env.IsDevelopment();
            if (isDevelopmentMode)
            {
                app.UseDeveloperExceptionPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseSerilogRequestLogging(options =>
            {
                options.EnrichDiagnosticContext = PushAdditionalSeriLogProperties;
            });
            app.UseWebSockets();

            app.UseResponseCompression();
            AddSecurityHeaders(app);

            app.UseStaticFiles();
            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            // Премахваме MaintenanceMode Middleware, тъй като не го използваме
            // app.UseMaintenanceModeMiddleware();
            app.UseHttpMetrics(options =>
            {
                // This will preserve only the first digit of the status code.
                // For example: 200, 201, 203 -> 2xx
                options.ReduceStatusCodeCardinality();
            });

            //app.UseMetricServer(9102);

            app.UseEndpoints(endpoints =>
            {
                // Endpoint за метрики отговаря на специфичен порт
                endpoints.MapMetrics().RequireHost("*:9102");

                endpoints.MapControllers();
                endpoints.MapHub<StudentHub>("/student-hub");
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

            if (HangfireRedisConnection != null)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHangfireDashboard();
                });

                // This middleware must be placed AFTER the authentication middlewares!
                app.UseHangfireDashboard("/background/hangfire", options: new DashboardOptions
                {
                    Authorization = new[] { new AdministratorHangfireDashboardAuthorizationFilter() }
                });
                HangfireJobScheduler.ScheduleRecurringJobs(app.ApplicationServices);
            }
        }

        public class ActionHidingConvention : IActionModelConvention
        {
            public void Apply(ActionModel action)
            {
                if (action.Controller.ControllerName.Contains("ReportDesigner"))
                {
                    action.ApiExplorer.IsVisible = false;
                }
            }
        }
    }
}

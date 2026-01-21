[assembly: Microsoft.AspNetCore.Mvc.ApiConventionType(typeof(SB.ApiAbstractions.SBApiConventions))]

namespace SB.ExtApi;

using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.AspNetCore;
using Prometheus;
using SB.ApiAbstractions;
using SB.Common;
using SB.Data;
using SB.Domain;
using Serilog;
using ZymLabs.NSwag.FluentValidation;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        this.Configuration = configuration;
        this.Environment = environment;
        this.ExtApiOptions = new ExtApiOptions();
        configuration.GetSection("SB:ExtApi").Bind(this.ExtApiOptions);

        this.Modules = new SBModule[]
        {
            new DomainModule(),
            new DataModule(),
        };
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; }

    public ExtApiOptions ExtApiOptions { get; }

    public SBModule[] Modules { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.RegisterModules(this.Configuration, this.Modules);

        services.Configure<ExtApiOptions>(this.Configuration.GetSection("SB:ExtApi"));

        services.AddCertificateForwarding(options =>
        {
            options.CertificateHeader = "X-Client-Cert";

            options.HeaderConverter = (headerValue) =>
            {
                X509Certificate2? clientCertificate = null;

                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    clientCertificate = X509Certificate2.CreateFromPem($"-----BEGIN CERTIFICATE-----{headerValue}-----END CERTIFICATE-----");
                }

                return clientCertificate!;
            };
        });

        services.AddTransient<ExtSystemClaimsPrincipalProvider>();
        services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
            .AddCertificate(
                CertificateAuthenticationDefaults.AuthenticationScheme,
                options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                    options.RevocationMode = X509RevocationMode.NoCheck;
                    options.ChainTrustValidationMode = X509ChainTrustMode.CustomRootTrust;
                    // TODO remove this line when the root certificate is renewed
                    options.ValidateValidityPeriod = false;
                    options.CustomTrustStore =
                        new X509Certificate2Collection(
                            X509Certificate2.CreateFromPem(
                                this.ExtApiOptions.NeispuoExtApiRootCaPem
                                    ?? throw new Exception("Missing NeispuoExtApiRootCaPem in configuration.")));
                    options.Events = new CertificateAuthenticationEvents
                    {
                        OnCertificateValidated = async context =>
                        {
                            var extSystemClaimsPrincipalProvider =
                                context.HttpContext.RequestServices.GetRequiredService<ExtSystemClaimsPrincipalProvider>();

                            var principal =
                                await extSystemClaimsPrincipalProvider.GetByCertificateThumbprintAsync(
                                    context.ClientCertificate.Subject,
                                    context.ClientCertificate.Thumbprint,
                                    context.Scheme.Name,
                                    context.HttpContext.RequestAborted);

                            if (principal != null)
                            {
                                context.Principal = principal;
                                context.Success();
                                return;
                            }

                            context.Fail("Certificate does not match any known ExtSystem.");
                        }
                    };
                });

        services
            .AddAuthorization(auth =>
            {
                var hisMedicalNoticesAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(CertificateAuthenticationDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(
                            new ExtSystemAccessRequirement
                            {
                                 Assertion = (ctx) =>
                                    ctx.ExtSystemId ==
                                        (ctx.Options.HisExtSystemId
                                            ?? throw new Exception("Missing HisExtSystemId in configuration."))
                                    && ctx.ExtSystemTypes.Contains(AuthorizationConstants.ExtSystemTypeExternalIntegration)
                            })
                        .Build();

                var medicalNoticesAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(CertificateAuthenticationDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(
                            new ExtSystemAccessRequirement
                            {
                                 Assertion = (ctx) => ctx.ExtSystemTypes.Contains(AuthorizationConstants.ExtSystemTypeSchoolBooks)
                            })
                        .Build();

                var instSchoolYearBookAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(CertificateAuthenticationDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new InstSchoolYearBookAccessRequirement())
                        .Build();

                var denyAllAccessPolicy = new AuthorizationPolicyBuilder()
                        .RequireAssertion(_ => false)
                        .Build();

                auth.AddPolicy(Policies.HisMedicalNoticesAccess, hisMedicalNoticesAccessPolicy);
                auth.AddPolicy(Policies.MedicalNoticesAccess, medicalNoticesAccessPolicy);
                auth.AddPolicy(Policies.InstSchoolYearBookAccess, instSchoolYearBookAccessPolicy);

                auth.DefaultPolicy = auth.FallbackPolicy = denyAllAccessPolicy;
            });

        services
            .AddScoped<IAuthorizationHandler, ExtSystemAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, InstSchoolYearBookAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, InstSchoolYearBookAccessRequirementScheduleHandler>()
            .AddSingleton<ILocalizationService, LocalizationService>()
            .AddScoped<ClassBookBelongsToInstitutionFilter>();

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.AddControllers(mvcOptions =>
        {
            mvcOptions.Conventions.Add(new RouteTokenTransformerConvention(new CamelCaseParameterTransformer()));
            mvcOptions.Filters.Add<DomainExceptionFilter>();
            mvcOptions.Filters.Add<ExtApiSqlExceptionWhitelistFilter>();
            mvcOptions.ModelBinderProviders.Insert(0, new AssumeLocalDateTimeModelBinderProvider());
            mvcOptions.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        });

        services.AddSingleton<IConfigureOptions<ForwardedHeadersOptions>>(serviceProvider =>
        {
            var apiOptions = serviceProvider.GetRequiredService<IOptions<ExtApiOptions>>().Value;

            return new ForwardedHeadersOptionsConfiguration(
                apiOptions.ForwardedHeadersKnownNetworks,
                apiOptions.ForwardedHeadersForwardLimit);
        });

        services.AddSingleton<FluentValidationSchemaProcessor>();

        services
            .AddOpenApiDocument((document, serviceProvider) =>
            {
                var apiOptions = serviceProvider.GetRequiredService<IOptions<ExtApiOptions>>().Value;
                var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
                var releaseNotesPath = new PathString(apiOptions.PathBase).Add("/api/v1/bg/documentation/release-notes");
                var businessProcessesPath = new PathString(apiOptions.PathBase).Add("/api/v1/bg/documentation/business-processes");
                document.GenerateEnumMappingDescription = true;
                document.DocumentName = "bg";
                document.SchemaType = SchemaType.OpenApi3;
                document.Title = "НЕИСПУО - Документи за институцията";
                document.Description = $"Подробна информация на API може да намерите на линка под заглавието.</br>" +
                                       $"Описание на промените, можете да намерите в <a href=\"{releaseNotesPath}\">release notes</a>.</br>" +
                                       $"Допълнителна информация отностно бизнес процеси може да намрите в <a href=\"{businessProcessesPath}\">business processes</a>.";
                document.Version = "1.0.22";
                document.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null;

                var fluentValidationSchemaProcessor = serviceProvider.GetRequiredService<FluentValidationSchemaProcessor>();
                document.SchemaProcessors.Add(fluentValidationSchemaProcessor);
                document.SchemaProcessors.Add(new NonNullablePropertiesAsRequiredSchemaProcessor());
                document.SchemaProcessors.Add(new XEnumVarnamesSchemaProcessor());
                document.SchemaProcessors.Add(new MultiLanguageProcessor("bg", localizationService));

                document.OperationProcessors.Add(new NonNullableParametersAsRequiredOperationProcessor());
                document.OperationProcessors.Add(new MultiLanguageProcessor("bg", localizationService));
            })
            .AddOpenApiDocument((document, serviceProvider) =>
            {
                var apiOptions = serviceProvider.GetRequiredService<IOptions<ExtApiOptions>>().Value;
                var localizationService = serviceProvider.GetRequiredService<ILocalizationService>();
                var releaseNotesPath = new PathString(apiOptions.PathBase).Add("/api/v1/en/documentation/release-notes");
                var businessProcessesPath = new PathString(apiOptions.PathBase).Add("/api/v1/en/documentation/business-processes");
                document.GenerateEnumMappingDescription = true;
                document.DocumentName = "en";
                document.SchemaType = SchemaType.OpenApi3;
                document.Title = "NEISPUO - Institution Documents";
                document.Description = $"Detailed API information can be found at the link below the title.</br>" +
                                       $"Description of changes can be found in <a href=\"{releaseNotesPath}\">release notes</a>.</br>" +
                                       $"Additional information regarding business processes can be found in <a href=\"{businessProcessesPath}\">business processes</a>.";
                document.Version = "1.0.22";
                document.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null;

                var fluentValidationSchemaProcessor = serviceProvider.GetRequiredService<FluentValidationSchemaProcessor>();
                document.SchemaProcessors.Add(fluentValidationSchemaProcessor);
                document.SchemaProcessors.Add(new NonNullablePropertiesAsRequiredSchemaProcessor());
                document.SchemaProcessors.Add(new XEnumVarnamesSchemaProcessor());
                document.SchemaProcessors.Add(new MultiLanguageProcessor("en", localizationService));

                document.OperationProcessors.Add(new NonNullableParametersAsRequiredOperationProcessor());
                document.OperationProcessors.Add(new MultiLanguageProcessor("en", localizationService));
            });

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(10);
            options.Period = TimeSpan.FromSeconds(10);
        });

        services.AddHealthChecks()
            .ForwardToPrometheus();
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModules(this.Configuration, this.Modules);

        builder.RegisterType<ExtApiRequestContext>().As<IRequestContext>().InstancePerLifetimeScope();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.Use((context, next) =>
        {
            context.Request.PathBase = new PathString(this.ExtApiOptions.PathBase);
            return next(context);
        });

        // health check branch
        app.MapWhen(httpContext => httpContext.Connection.LocalPort == 9101, branchedApp => {
            branchedApp.UseHealthChecks("/healthz");
        });

        // metrics branch
        app.MapWhen(httpContext => httpContext.Connection.LocalPort == 9102, branchedApp => {
            branchedApp.UseMetricServer("/metrics");
        });

        // main branch
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = LoggingHostBuilderExtensions.SerilogRequestLoggingMessageTemplate;
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("IpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("ActionName", httpContext.GetEndpoint()?.DisplayName);
                diagnosticContext.Set("SysUserId", httpContext.GetSysUserId());
            };
        });

        app.UseForwardedHeaders();
        app.UseCertificateForwarding();

        app.UseOpenApi(document => {
            document.DocumentName = "bg";
            document.PostProcess = (document, request) =>
            {
                document.BasePath = request.PathBase.Add("/api/v1");
            };
        });

        app.UseOpenApi(document => {
            document.DocumentName = "en";
            document.PostProcess = (document, request) =>
            {
                document.BasePath = request.PathBase.Add("/api/v1");
            };
        });

        app.UseSwaggerUi3(settings =>
        {
            settings.SwaggerRoutes.Add(new SwaggerUi3Route("BG", "/swagger/bg/swagger.json"));
            settings.SwaggerRoutes.Add(new SwaggerUi3Route("EN", "/swagger/en/swagger.json"));
        });

        app.Map("/api/v1", app =>
        {
            app.UseRouting();

            app.UseHttpMetrics();
            // convert exceptions to StatusCode:500 just after UseHttpMetrics
            // as we want them correctly tracked in Prometheus
            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseWhen(
                context =>
                    (context.GetRouteValue("controller") as string) != nameof(DocumentationController).GetControllerName() ||
                    (context.GetRouteValue("action") as string) != nameof(DocumentationController.GetReleaseNotesAsync),
                app => app.UseSecurityHeaders());

            app.UseRequestIdHeader();

            app.Use(async (context, next) =>
            {
                var currentEndpoint = context.GetEndpoint();

                if (currentEndpoint is null && !context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    await context.Response.WriteAsync("The requested resource could not be found. Check your url.");
                    return;
                }

                await next(context);
                return;
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        });
    }
}

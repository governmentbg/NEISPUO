[assembly: Microsoft.AspNetCore.Mvc.ApiConventionType(typeof(SB.ApiAbstractions.SBApiConventions))]

namespace SB.Api;

using System;
using System.Text.Json.Serialization;
using Autofac;
using Invio.Extensions.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag;
using NSwag.Generation.Processors.Security;
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
        this.ApiOptions = new ApiOptions();
        configuration.GetSection("SB:Api").Bind(this.ApiOptions);

        this.Modules = new SBModule[]
        {
            new DomainModule(),
            new DataModule(),
        };
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; }

    public ApiOptions ApiOptions { get; }

    public SBModule[] Modules { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.RegisterModules(this.Configuration, this.Modules);

        services.Configure<ApiOptions>(this.Configuration.GetSection("SB:Api"));

        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                (options) =>
                {
                    options.Authority = this.ApiOptions.OIDCAuthority ?? throw new Exception($"Missing {nameof(this.ApiOptions.OIDCAuthority)}");
                    options.Audience = this.ApiOptions.OIDCAudience ?? throw new Exception($"Missing {nameof(this.ApiOptions.OIDCAudience)}");
                    options.RequireHttpsMetadata = this.ApiOptions.OIDCRequireHttpsMetadata ?? !this.Environment.IsDevelopment();
                    options.TokenValidationParameters.ClockSkew = this.ApiOptions.OIDCClockSkew ?? TokenValidationParameters.DefaultClockSkew;
                })
            .AddJwtBearerQueryStringAuthentication(
                (options) =>
                {
                    options.QueryStringParameterName = "access_token";
                    options.QueryStringBehavior = QueryStringBehaviors.Redact;
                });

        services
            .AddAuthorization(auth =>
            {
                var institutionAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new InstitutionAccessRequirement())
                        .Build();

                var reportAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new ReportAccessRequirement())
                        .Build();

                var institutionAdminAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new InstitutionAdminAccessRequirement())
                        .Build();

                var classBookAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new ClassBookAccessRequirement())
                        .Build();

                var classBookAdminAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new ClassBookAdminAccessRequirement())
                        .Build();

                var studentInfoClassBookAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new StudentInfoClassBookAccessRequirement())
                        .Build();

                var curriculumAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new CurriculumAccessRequirement())
                        .Build();

                var scheduleLessonAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new ScheduleLessonAccessRequirement())
                        .Build();

                var supportAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new SupportAccessRequirement())
                        .Build();

                var attendanceDateAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new AttendanceDateAccessRequirement())
                        .Build();

                var hisMedicalNoticeAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new HisMedicalNoticeAccessRequirement())
                        .Build();

                var studentAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new StudentAccessRequirement())
                        .Build();

                var studentClassBookAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new StudentClassBookAccessRequirement())
                        .Build();

                var studentMedicalNoticesAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .AddRequirements(new StudentMedicalNoticesAccessRequirement())
                        .Build();

                var authenticatedAccessPolicy = new AuthorizationPolicyBuilder()
                        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .Build();

                var denyAllPolicy = new AuthorizationPolicyBuilder()
                        .AddRequirements(new DenyAllRequirement())
                        .Build();

                var conversationAccessPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ConversationAccessRequirement())
                    .Build();


                auth.AddPolicy(Policies.InstitutionAccess, institutionAccessPolicy);
                auth.AddPolicy(Policies.ReportAccess, reportAccessPolicy);
                auth.AddPolicy(Policies.InstitutionAdminAccess, institutionAdminAccessPolicy);
                auth.AddPolicy(Policies.ClassBookAccess, classBookAccessPolicy);
                auth.AddPolicy(Policies.StudentInfoClassBookAccess, studentInfoClassBookAccessPolicy);
                auth.AddPolicy(Policies.ClassBookAdminAccess, classBookAdminAccessPolicy);
                auth.AddPolicy(Policies.CurriculumAccess, curriculumAccessPolicy);
                auth.AddPolicy(Policies.ScheduleLessonAccess, scheduleLessonAccessPolicy);
                auth.AddPolicy(Policies.SupportAccess, supportAccessPolicy);
                auth.AddPolicy(Policies.AttendanceDateAccess, attendanceDateAccessPolicy);
                auth.AddPolicy(Policies.HisMedicalNoticeAccess, hisMedicalNoticeAccessPolicy);

                auth.AddPolicy(Policies.StudentAccess, studentAccessPolicy);
                auth.AddPolicy(Policies.StudentClassBookAccess, studentClassBookAccessPolicy);
                auth.AddPolicy(Policies.StudentMedicalNoticesAccess, studentMedicalNoticesAccessPolicy);
                auth.AddPolicy(Policies.ConversationAccess, conversationAccessPolicy);


                auth.AddPolicy(Policies.AuthenticatedAccess, authenticatedAccessPolicy);
                auth.AddPolicy(Policies.DenyAll, denyAllPolicy);

                // any action not explicitly decorated with [Authorize(Policy = "<some other policy>")] or [AllowAnonymous] will use the DenyAll policy
                auth.DefaultPolicy = auth.FallbackPolicy = denyAllPolicy;
            });

        services
            .AddScoped<IAuthorizationHandler, InstitutionAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, ReportAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, InstitutionAdminAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, ClassBookAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, StudentInfoClassBookAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, ClassBookAdminAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, CurriculumAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, ScheduleLessonAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, SupportAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, AttendanceDateAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, HisMedicalNoticeAccessRequirementHandler>()

            .AddScoped<IAuthorizationHandler, StudentAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, StudentClassBookAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, StudentMedicalNoticesAccessRequirementHandler>()
            .AddScoped<IAuthorizationHandler, ConversationAccessRequirementHandler>()

            .AddScoped<IAuthorizationHandler, DenyAllRequirementHandler>();

        services
            .AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            })
            .AddControllers(mvcOptions =>
            {
                mvcOptions.Filters.Add<DomainExceptionFilter>();
                mvcOptions.Filters.Add<ApiSqlExceptionWhitelistFilter>();
                mvcOptions.ModelBinderProviders.Insert(0, new AssumeLocalDateTimeModelBinderProvider());
            })
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter());
            });

        services.AddSingleton<IConfigureOptions<ForwardedHeadersOptions>>(serviceProvider =>
        {
            var apiOptions = serviceProvider.GetRequiredService<IOptions<ApiOptions>>().Value;

            return new ForwardedHeadersOptionsConfiguration(
                apiOptions.ForwardedHeadersKnownNetworks,
                apiOptions.ForwardedHeadersForwardLimit);
        });

        if (this.Environment.IsDevelopment())
        {
            services.AddSingleton<FluentValidationSchemaProcessor>();
            services.AddOpenApiDocument((document, serviceProvider) =>
            {
                document.SchemaType = SchemaType.OpenApi3;
                document.DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null;
                document.SchemaNameGenerator = new ApiSchemaNameGenerator();
                document.AddSecurity(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Bearer",
                        Description = "Enter JWT Bearer token **_only_**",
                        In = OpenApiSecurityApiKeyLocation.Header,
                        Type = OpenApiSecuritySchemeType.Http,
                        Scheme = "bearer", // must be lower case
                        BearerFormat = "JWT",
                    });

                document.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));

                var fluentValidationSchemaProcessor = serviceProvider.GetRequiredService<FluentValidationSchemaProcessor>();
                document.SchemaProcessors.Add(fluentValidationSchemaProcessor);
                document.SchemaProcessors.Add(new NonNullablePropertiesAsRequiredSchemaProcessor());
                document.SchemaProcessors.Add(new XEnumVarnamesSchemaProcessor());

                document.OperationProcessors.Add(new NonNullableParametersAsRequiredOperationProcessor());
                document.OperationProcessors.Add(new TextPlainAsStringOperationProcessor());
            });
        }

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader();

                    builder.WithOrigins(
                        this.ApiOptions.AllowedCorsOrigins
                        ?? Array.Empty<string>());

                    builder.WithExposedHeaders(RequestIdHeaderMiddleware.RequestIdHeaderName);
                });
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

        builder.RegisterType<ApiRequestContext>().As<IRequestContext>().InstancePerLifetimeScope();
        builder.RegisterType<AuthService>().As<IAuthService>().InstancePerLifetimeScope();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }

        // health check branch
        app.MapWhen(httpContext => httpContext.Connection.LocalPort == 9101, branchedApp =>
        {
            branchedApp.UseHealthChecks("/healthz");
        });

        // metrics branch
        app.MapWhen(httpContext => httpContext.Connection.LocalPort == 9102, branchedApp =>
        {
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
                diagnosticContext.Set("SessionId", httpContext.GetSessionId());
            };
        });

        app.UseForwardedHeaders();

        app.UseRouting();

        app.UseCors();

        app.UseHttpMetrics();
        // convert exceptions to StatusCode:500 just after UseHttpMetrics
        // as we want them correctly tracked in Prometheus
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseSecurityHeaders();
        app.UseRequestIdHeader();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseJwtBearerQueryString();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

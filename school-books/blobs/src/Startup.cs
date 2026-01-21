namespace SB.Blobs;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Serilog;

public class Startup
{
    public static readonly string RequestIdHeaderName = "X-Sb-Request-Id";

    private BlobsOptions BlobsOptions { get; init; }
    private DataOptions DataOptions { get; init; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        this.Configuration = configuration;
        this.Environment = environment;

        this.BlobsOptions = new BlobsOptions();
        configuration.GetSection("SB:Blobs").Bind(this.BlobsOptions);
        this.DataOptions = new DataOptions();
        configuration.GetSection("SB:Data").Bind(this.DataOptions);
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<BlobsOptions>(this.Configuration.GetSection("SB:Blobs"));
        services.Configure<DataOptions>(this.Configuration.GetSection("SB:Data"));

        services.AddSingleton<IActionResultExecutor<BlobStreamResult>, BlobStreamResultExecutor>();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.Configure<HealthCheckPublisherOptions>(options =>
        {
            options.Delay = TimeSpan.FromSeconds(10);
            options.Period = TimeSpan.FromSeconds(10);
        });

        services.AddHealthChecks()
            .AddSqlServer(this.DataOptions.GetConnectionString())
            .ForwardToPrometheus();

        if (this.BlobsOptions.HasPostAuthentication)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    (options) =>
                    {
                        options.Authority = this.BlobsOptions.PostAuthOIDCAuthority ?? throw new Exception($"Missing {nameof(this.BlobsOptions.PostAuthOIDCAuthority)}");
                        options.TokenValidationParameters.ValidAudiences = this.BlobsOptions.PostAuthOIDCValidAudiences ?? throw new Exception($"Missing {nameof(this.BlobsOptions.PostAuthOIDCValidAudiences)}");
                        options.TokenValidationParameters.ClockSkew = this.BlobsOptions.PostAuthOIDCClockSkew ?? TokenValidationParameters.DefaultClockSkew;
                        options.RequireHttpsMetadata = this.BlobsOptions.PostAuthOIDCRequireHttpsMetadata ?? !this.Environment.IsDevelopment();
                    });
        }

        services
            .AddControllers()
            .AddJsonOptions(jsonOptions =>
            {
                jsonOptions.JsonSerializerOptions.Converters.Add(
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            });

        services.AddSingleton<IConfigureOptions<ForwardedHeadersOptions>>(serviceProvider =>
        {
            var blobsOptions = serviceProvider.GetRequiredService<IOptions<BlobsOptions>>().Value;

            return new ForwardedHeadersOptionsConfiguration(
                blobsOptions.ForwardedHeadersKnownNetworks,
                blobsOptions.ForwardedHeadersForwardLimit);
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    builder.AllowAnyMethod()
                        .AllowAnyHeader();

                    if (this.BlobsOptions.AllowedCorsOrigins?.Length > 0)
                    {
                        builder.WithOrigins(this.BlobsOptions.AllowedCorsOrigins);
                    }
                    else
                    {
                        builder.AllowAnyOrigin();
                    }

                    builder.WithExposedHeaders(RequestIdHeaderName);
                });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
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
            // change default message template to remove "Elapsed" property rendering
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("IpAddress", httpContext.Connection.RemoteIpAddress?.ToString());
                diagnosticContext.Set("ActionName", httpContext.GetEndpoint()?.DisplayName);
                diagnosticContext.Set("SysUserId", httpContext.GetSelectedRole()?.SysUserId);
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

        app.Use(async (context, next) =>
        {
            context.Response.OnStarting(() =>
            {
                // add the security headers recommended for backend apis by Mozilla
                // https://observatory.mozilla.org/faq/
                // the HSTS header is not added as SSL is offloaded by the loadbalancer
                context.Response.Headers.Add("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none';");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

                // add RequestId header
                context.Response.Headers.Add(RequestIdHeaderName, context.TraceIdentifier);

                return Task.CompletedTask;
            });

            await next();
        });

        if (this.BlobsOptions.HasPostAuthentication)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}

namespace MonProjects.Infrastructure.Extensions
{
    using Configurations;
    using Microsoft.AspNetCore.Authentication.Certificate;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;
    using Services;
    using Services.CertificateValidate;
    using Services.ExtSystem;
    using Services.Logs;
    using System;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppSettings(this IServiceCollection services,
                IConfiguration configuration)
            => services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

        public static IServiceCollection AddCertificateAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                    .AddCertificate();

            return services;
        }

        public static IServiceCollection AddCertificateForwarding(this IServiceCollection services)
        {
            services
                .AddCertificateForwarding(options =>
                {
                    options.CertificateHeader = "X-Client-Cert";
                    options.HeaderConverter = (headerValue) =>
                    {
                        X509Certificate2 clientCertificate = null;

                        if (!string.IsNullOrWhiteSpace(headerValue))
                        {
                            var bytes = UrlPemToByteArray(headerValue);
                            clientCertificate = new X509Certificate2(bytes);
                        }

                        return clientCertificate;
                    };
                });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
            => services
                .AddTransient<IMsSqlNeispuoService, MsSqlNeispuoService>()
                .AddTransient<ILogService, LogService>()
                .AddTransient<IExtSystemServices, ExtSystemServices>()
                .AddTransient<ICertificateValidateService, CertificateValidateService>();

        public static IServiceCollection AddSwagger(this IServiceCollection services)
           => services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc(
                   "v1",
                   new OpenApiInfo
                   {
                       Title = $"{nameof(MonProjects)} Api",
                       Version = "v1"
                   });
               c.AddSecurityDefinition("X-Client-Cert", new OpenApiSecurityScheme
               {
                   In = ParameterLocation.Header,
                   Description = "",
                   Name = "X-Client-Cert",
                   Type = SecuritySchemeType.ApiKey
               });
               c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "X-Client-Cert"
                        }
                    },
                    new string[] { }
                }
               });
               c.EnableAnnotations();
           });

        private static byte[] UrlPemToByteArray(string urlBase64Pem)
            => Convert.FromBase64String(urlBase64Pem
                                        .Replace("-----BEGIN CERTIFICATE-----", string.Empty)
                                        .Replace("-----END CERTIFICATE-----", string.Empty)
                                        .Trim());

    }
}

namespace NeispuoExtension.Infrastructure.Extensions
{
    using System.Threading.Tasks;

    using Microsoft.OpenApi.Models;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.AspNetCore.Authentication.JwtBearer;

    using DependencyInjection.Extensions;

    using Settings;
    using Settings.Authentication;

    using Services.Core;
    using Database.Services;

    public static class ServiceCollectionExtensions
    {
        public static ApplicationSettings ConfigureSettings(this IServiceCollection services, IConfiguration configuration)
        {
            IConfigurationSection settingsSection = configuration.GetSection(nameof(ApplicationSettings));

            services.Configure<ApplicationSettings>(options => settingsSection.Bind(options));

            return settingsSection.Get<ApplicationSettings>();
        }

        public static IServiceCollection AddAuthentication(this IServiceCollection services, IAuthenticationSettings authenticationSettings)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.Audience = authenticationSettings.Audience;
                    options.Authority = authenticationSettings.Authority;

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }

                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
             => services.AddDependencyInjection(
                 typeof(CoreServicesAssemblyEntry).Assembly,
                 typeof(DatabaseServicesAssemblyEntry).Assembly);

        public static IServiceCollection AddSwagger(this IServiceCollection services)
            => services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(
                    "v1",
                    new OpenApiInfo
                    {
                        Title = "DependencyInjection API",
                        Version = "v1"
                    });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
                });
                c.EnableAnnotations();
            });
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MON.API.Security;
using MON.Services.Security.Permissions;

namespace MON.API.Identity
{
    public static class AuthenticationUtil
    {
        /// <summary>
        /// API-то се защитава чрез JWT bearer token. Token-ът може да се предостави от IdentityServer, макар че това не е задължително.
        /// </summary>
        public static void AddApiAuthentication(this IServiceCollection services, AuthenticationConfig authConfig)
        {
            // GDPR настройка, която по подразбиране крие някакви детайли от логовете. PII означава personally identifiable information.
            Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents()
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return System.Threading.Tasks.Task.CompletedTask;
                        },

                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/student-hub"))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return System.Threading.Tasks.Task.CompletedTask;
                        }

                    };
                    // Не изисква https
                    options.RequireHttpsMetadata = false;
                    // base-address of your identityserver
                    options.Authority = authConfig.IdentityServerUrl;
                    // name of the API resource
                    options.Audience = authConfig.ApiName;

                    options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        //RoleClaimType = "role",
                        //ValidateIssuer = true,
                        //ValidateAudience = true,
                        //ValidateIssuerSigningKey = true,
                    };
                });


            services.AddAuthorization(options =>
            {
                foreach (var permission in DefaultPermissions.GetAll())
                {
                    options.AddPolicy(permission.Name,
                        policy => policy.Requirements.Add(new PermissionRequirement(permission)));
                }
            });

            // Asp.net има вградена поддръжка за защита на API чрез JWT, но авторите на IdentityServer рекламират подобрен вариант,
            // който поддържа JWT, reference tokens и валидиране на token-ите.
            // Изисква пакет IdentityServer4.AccessTokenValidation (виж http://docs.identityserver.io/en/latest/topics/apis.html).

            //services.AddAuthentication(IdentityServer4.AccessTokenValidation.IdentityServerAuthenticationDefaults.AuthenticationScheme)
            //    .AddIdentityServerAuthentication(options =>
            //    {
            //        options.Authority = authConfig.IdentityServerUrl.TrimEnd('/');
            //        options.ApiName = authConfig.ApiName;
            //    });

            // * По подразбиране метаданните на IdentityServer не могат да се четат по http,
            // т.е. http://localhost:8043/.well-known/openid-configuration хвърля internal server error.
            // За dev цели тази забрана може да се изключи по-горе в опциите, макар че в този проект това няма смисъл.
            //options.Authority = "http://localhost:8043";
            //options.RequireHttpsMetadata = false;
        }

        //public const string SpaDevServerToApiCorsPolicy = "SpaDevServerToApi";

        //public static void AddApiCors(this IServiceCollection services, SpaDevServerConfig spaDevServerConfig)
        //{
        //    services.AddCors(options =>
        //    {
        //        // Тази политика се включва само по време на разработка. Разрешава POST заявките от SPA dev сървъра към API-то.
        //        if (spaDevServerConfig != null)  // Dev mode.
        //        {
        //            options.AddPolicy(SpaDevServerToApiCorsPolicy, policy =>
        //            {
        //                policy.WithOrigins(spaDevServerConfig.Origin).AllowAnyHeader().AllowAnyMethod();
        //            });
        //        }
        //    });
        //}
    }
}

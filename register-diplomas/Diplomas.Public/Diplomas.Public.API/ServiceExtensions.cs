using Microsoft.Extensions.DependencyInjection;

namespace Diplomas.Public.API
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services, string[] allowedOrigins)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace MON.API
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
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition"));
            });
        }
    }
}

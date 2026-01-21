namespace NeispuoExtension.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Builder;

    using Middlewares;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseNeispuoExtensionUserIdentity(this IApplicationBuilder app)
             => app.UseMiddleware<UserIdentityMiddleware>();

        public static IApplicationBuilder UseNeispuoExtensionExceptionHandler(this IApplicationBuilder app)
             => app.UseMiddleware<ExceptionHandlerMiddleware>();

        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
            => app
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "NeispuoExtension API");
                    options.RoutePrefix = string.Empty;
                });
    }
}

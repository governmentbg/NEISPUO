namespace MonProjects.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Builder;
    using MonProjects.Infrastructure.Middlewares;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
            => app
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{nameof(MonProjects)} Api");
                    options.RoutePrefix = string.Empty;
                });

        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        public static IApplicationBuilder UseCertificateAuthentication(this IApplicationBuilder app)
            => app.UseMiddleware<CertificateAuthenticationMiddleware>();

        public static IApplicationBuilder UseProjectQueryStringValidation(this IApplicationBuilder app)
            => app.UseMiddleware<ProjectQueryStringValidationMiddleware>();

    }
}

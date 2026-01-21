namespace NeispuoExtension.Api
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Settings;

    using Infrastructure.Extensions;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ApplicationSettings settings = services
                .ConfigureSettings(this.configuration);

            services
                .AddAuthentication(settings.Authentication)
                .AddServices()
                .AddSwagger()
                .AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseNeispuoExtensionExceptionHandler();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage()
                    .UseSwaggerUI();
            }

            app.UseHttpsRedirection()
                .UseRouting();

            app.UseCors(options =>
            {
                options.AllowAnyOrigin();
                options.AllowAnyHeader();
                options.AllowAnyMethod();
            });

            app.UseAuthentication()
                .UseAuthorization()
                .UseNeispuoExtensionUserIdentity();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

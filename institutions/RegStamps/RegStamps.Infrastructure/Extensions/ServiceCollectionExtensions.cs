namespace RegStamps.Infrastructure.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        //public static IServiceCollection AddAppSettings(this IServiceCollection services, IConfiguration configuration)
        //    => services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));


        //public static IServiceCollection AddDbContextData(this IServiceCollection services, IConfiguration configuration)
        //    => services.AddDbContext<DataStampsContext>(options =>
        //        options.UseSqlServer(configuration.GetConnectionString("AppSettings:Database:DataStampsDatabase:ConnectionString")));

        //public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        //    => services
        //        .AddTransient<IUserStore<ApplicationUser>, UserStoreService>()
        //        .AddTransient<IRoleStore<ApplicationRole>, RoleStoreService>();
    }
}

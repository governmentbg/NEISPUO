namespace SB.Data;

using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SB.Common;
using SB.Domain;
using System.Linq;
using System.Reflection;

public class DataModule : SBModule
{
    public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<DbContext>(
            (serviceProvider, optionsBuilder) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<DataOptions>>().Value;
                optionsBuilder.UseSqlServer(
                    options.GetConnectionString());

                    // TODO consider adding Throw on MultipleCollectionIncludeWarning as its a good practice
                    // to avoid N*M queries and always use AsSplitQuery in queries with more than one principal,
                    // e.g. not filtered by a single id (for those we should use AsSingleQuery)
                    // .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning));

                if (options.EnableSensitiveDataLogging)
                {
                    optionsBuilder.EnableSensitiveDataLogging();
                }

            },
            ServiceLifetime.Singleton);

        services.AddHealthChecks()
            .AddCheck<DbContextHealthCheck>(nameof(DbContextHealthCheck));

        services.Configure<DataOptions>(configuration.GetSection("SB:Data"));
    }

    public override void ConfigureAutofacServices(ContainerBuilder builder, IConfiguration configuration)
    {
        var dataAssembly = typeof(DataModule).GetTypeInfo().Assembly;

        builder.RegisterAssemblyTypes(dataAssembly)
            .Where(t => t.Name.EndsWith("Repository"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();

        builder
            .RegisterAssemblyTypes(dataAssembly)
            .Where(type =>
                !type.GetTypeInfo().IsAbstract &&
                type.GetInterfaces()
                    .Any(i => i.Equals(typeof(IEntityMapping))))
            .As<IEntityMapping>();

        builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().AsSelf().InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(AggregateRepository<>)).As(typeof(IAggregateRepository<>)).InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(ScopedAggregateRepository<>)).As(typeof(IScopedAggregateRepository<>)).InstancePerLifetimeScope();
        builder.RegisterGeneric(typeof(EnumNomsRepository<>)).As(typeof(IEnumNomsRepository<>)).InstancePerLifetimeScope();
    }
}

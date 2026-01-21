namespace NeispuoExtension.DependencyInjection.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Collections.Generic;

    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDependencyInjection(this IServiceCollection services, params Assembly[] assemblies)
        {
            void action(Assembly assembly)
            {
                // Register transient services
                _ = assembly.GetTypesByService(typeof(ITransientService))
                    .Select(type => services.AddTransient(type.Value, type.Key)).ToList();

                // Register scoped services
                _ = assembly.GetTypesByService(typeof(IScopedService))
                    .Select(type => services.AddScoped(type.Value, type.Key)).ToList();

                // Register singleton services
                _ = assembly.GetTypesByService(typeof(ISingletonService))
                    .Select(type => services.AddSingleton(type.Value, type.Key)).ToList();
            }
            assemblies.ToList().ForEach(action);

            services.AddTransient(typeof(Lazy<>), typeof(Lazier<>));

            return services;
        }

        private static IDictionary<Type, Type> GetTypesByService(this Assembly assembly, Type type)
            => assembly
                .GetExportedTypes()
                .Where(x => x.IsClass && !x.IsAbstract && type.IsAssignableFrom(x))
                .Select(type => new
                {
                    Implementation = type,
                    Service = type.GetInterface($"I{type.Name}")
                })
                .Where(t => t.Service != null)
                .ToDictionary(key => key.Implementation, value => value.Service);
    }
}

namespace SB.Domain;

using System;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion.Plugins;

public static class FusionCacheMetricsServiceCollectionExtensions
{
    public static IServiceCollection AddFusionCacheMetrics(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        services.AddSingleton<IFusionCachePlugin, FusionCacheMetricsPlugin>();

        return services;
    }
}

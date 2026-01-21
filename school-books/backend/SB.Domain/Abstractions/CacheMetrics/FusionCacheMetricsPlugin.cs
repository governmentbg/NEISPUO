namespace SB.Domain;

using Microsoft.Extensions.Caching.Memory;
using Prometheus;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Events;
using ZiggyCreatures.Caching.Fusion.Plugins;

public class FusionCacheMetricsPlugin : IFusionCachePlugin
{
    private static Counter HitsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_hits_total",
        "FusionCache item hit counter");
    private static Counter StaleHitsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_stale_hits_total",
        "FusionCache item stale hit counter (cache item failed to complete within soft timeout period)");
    private static Counter MissesTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_misses_total",
        "FusionCache item miss counter (when a cache item is not found in local cache)");
    private static Counter SetOperationsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_set_operations_total",
        "FusionCache item set operations counter (when a cache item is written to local cache)");
    private static Counter RemoveOperationsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_remove_operations_total",
        "FusionCache item remove operations counter (when a cache item is explicitly removed by user code)");
    private static Counter ExpiredItemsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_expired_items_total",
        "FusionCache item expired counter");
    private static Counter EvictedItemsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_evicted_items_total",
        "FusionCache item evicted due to capacity counter");
    private static Counter BackgroundRefreshesTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_background_refreshes_total",
        "FusionCache item refresh in background");
    private static Counter BackgroundRefreshFailuresTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_background_refresh_failures_total",
        "FusionCache item refresh in background failed");
    private static Counter FactoryErrorsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_factory_errors_total",
        "FusionCache factory errors counter");
    private static Counter FactorySyntheticTimeoutsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_factory_synthetic_timeouts_total",
        "FusionCache factory synthetic timeouts counter");
    private static Counter FailSafeActivationsTotal { get; set; } = Metrics.CreateCounter(
        "fusioncache_fail_safe_activations_total",
        "FusionCache fail-safe activations counter");
    private static Gauge MemoryCacheCurrentEntryCount { get; set; } = Metrics.CreateGauge(
        "fusioncache_memory_cache_current_entry_count",
        "FusionCache memory cache current entry count gauge");
    private static Gauge MemoryCacheCurrentEstimatedSize { get; set; } = Metrics.CreateGauge(
        "fusioncache_memory_cache_current_estimated_size",
        "FusionCache memory cache current estimated size gauge");
    private static Counter MemoryCacheTotalHits { get; set; } = Metrics.CreateCounter(
        "fusioncache_memory_cache_total_hits",
        "FusionCache memory cache total hits counter");
    private static Counter MemoryCacheTotalMisses { get; set; } = Metrics.CreateCounter(
        "fusioncache_memory_cache_total_misses",
        "FusionCache memory cache total misses counter");

    public FusionCacheMetricsPlugin(IMemoryCache? memoryCache = null)
    {
        if (memoryCache is MemoryCache cache)
        {
            Metrics.DefaultRegistry.AddBeforeCollectCallback(() =>
            {
                var statistics = memoryCache.GetCurrentStatistics();
                if (statistics is null)
                {
                    return;
                }

                MemoryCacheCurrentEntryCount.Set(statistics.CurrentEntryCount);
                MemoryCacheCurrentEstimatedSize.Set(statistics.CurrentEstimatedSize ?? 0);
                MemoryCacheTotalHits.IncTo(statistics.TotalHits);
                MemoryCacheTotalMisses.IncTo(statistics.TotalMisses);
            });
        }
    }

    public void Start(IFusionCache cache)
    {
        cache.Events.Hit += this.HandleCacheHit;
        cache.Events.Miss += this.HandleCacheMiss;
        cache.Events.Set += this.HandleCacheSet;
        cache.Events.Remove += this.HandleCacheRemove;
        cache.Events.Memory.Eviction += this.HandleCacheEviction;
        cache.Events.BackgroundFactorySuccess += this.HandleBackgroundFactorySuccess;
        cache.Events.BackgroundFactoryError += this.HandleBackgroundFactoryError;
        cache.Events.FactoryError += this.HandleFactoryError;
        cache.Events.FactorySyntheticTimeout += this.HandleFactorySyntheticTimeout;
        cache.Events.FailSafeActivate += this.HandleFailSafeActivate;
    }

    public void Stop(IFusionCache cache)
    {
        cache.Events.Hit -= this.HandleCacheHit;
        cache.Events.Miss -= this.HandleCacheMiss;
        cache.Events.Set -= this.HandleCacheSet;
        cache.Events.Remove -= this.HandleCacheRemove;
        cache.Events.Memory.Eviction -= this.HandleCacheEviction;
        cache.Events.BackgroundFactorySuccess -= this.HandleBackgroundFactorySuccess;
        cache.Events.BackgroundFactoryError -= this.HandleBackgroundFactoryError;
        cache.Events.FactoryError -= this.HandleFactoryError;
        cache.Events.FactorySyntheticTimeout -= this.HandleFactorySyntheticTimeout;
        cache.Events.FailSafeActivate -= this.HandleFailSafeActivate;
    }

    private void HandleCacheHit(object? sender, FusionCacheEntryHitEventArgs e)
    {
        if (!e.IsStale)
        {
            HitsTotal.Inc();
        }
        else
        {
            StaleHitsTotal.Inc();
        }
    }

    private void HandleCacheMiss(object? sender, FusionCacheEntryEventArgs e)
    {
        MissesTotal.Inc();
    }

    private void HandleCacheSet(object? sender, FusionCacheEntryEventArgs e)
    {
        SetOperationsTotal.Inc();
    }

    private void HandleCacheRemove(object? sender, FusionCacheEntryEventArgs e)
    {
        RemoveOperationsTotal.Inc();
    }

    private void HandleCacheEviction(object? sender, FusionCacheEntryEvictionEventArgs e)
    {
        switch (e.Reason)
        {
            case EvictionReason.Expired:

                ExpiredItemsTotal.Inc();
                break;

            case EvictionReason.Capacity:

                EvictedItemsTotal.Inc();
                break;
        }
    }

    private void HandleBackgroundFactorySuccess(object? sender, FusionCacheEntryEventArgs e)
    {
        BackgroundRefreshesTotal.Inc();
    }

    private void HandleBackgroundFactoryError(object? sender, FusionCacheEntryEventArgs e)
    {
        BackgroundRefreshFailuresTotal.Inc();
    }

    private void HandleFactoryError(object? sender, FusionCacheEntryEventArgs e)
    {
        FactoryErrorsTotal.Inc();
    }

    private void HandleFactorySyntheticTimeout(object? sender, FusionCacheEntryEventArgs e)
    {
        FactorySyntheticTimeoutsTotal.Inc();
    }

    private void HandleFailSafeActivate(object? sender, FusionCacheEntryEventArgs e)
    {
        FailSafeActivationsTotal.Inc();
    }
}

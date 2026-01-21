namespace SB.Domain;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;
using static SB.Domain.ICommonCQSQueryRepository;

class CommonRedisRepository : ICommonRedisRepository
{
    private const string CommonCacheKeyPrefix = "Common";
    private const string AllExtProvidersKey = $"{CommonCacheKeyPrefix}:AllExtProviders";
    private const string AllScheduleExtProvidersKey = $"{CommonCacheKeyPrefix}:AllScheduleExtProviders";
    private const int NoExtProviderId = -1;

    private IRedisConnectionMultiplexerAccessor connectionMultiplexerAccessor;

    public CommonRedisRepository(IRedisConnectionMultiplexerAccessor connectionMultiplexerAccessor)
    {
        this.connectionMultiplexerAccessor = connectionMultiplexerAccessor;
    }

    public async Task<bool?> GetInstHasCBExtProviderAsync(int schoolYear, int instId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();
        int? result = (int?)await db.HashGetAsync($"{AllExtProvidersKey}:{schoolYear}", instId);

        return result switch
        {
            NoExtProviderId => false,
            null => null,
            _ => true
        };
    }

    public async Task<bool?> GetExtSystemIsInstCBExtProviderAsync(int extSystemId, int schoolYear, int instId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return ((int?)await db.HashGetAsync($"{AllExtProvidersKey}:{schoolYear}", instId))?.Equals(extSystemId);
    }

    public async Task<bool?> GetExtSystemIsInstScheduleExtProviderAsync(int extSystemId, int schoolYear, int instId, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        return ((int?)await db.HashGetAsync($"{AllScheduleExtProvidersKey}:{schoolYear}", instId))?.Equals(extSystemId);
    }

    public async Task CacheAllExtProvidersAsync(int schoolYear, GetAllExtProvidersVO[] extProviders, TimeSpan exp, CancellationToken _)
    {
        IDatabase db = this.GetDatabase();

        await db.HashSetAsync(
            $"{AllExtProvidersKey}:{schoolYear}",
            extProviders.Select(p => new HashEntry(p.InstId, p.ExtSystemId ?? NoExtProviderId)).ToArray());
        await db.HashSetAsync(
            $"{AllScheduleExtProvidersKey}:{schoolYear}",
            extProviders.Select(p => new HashEntry(p.InstId, p.ScheduleExtSystemId ?? NoExtProviderId)).ToArray());

        // set an Expiry only on newly created keys as otherwise
        // the cashed hash can be held perpetually(moving the expiration back over and over)
        // since there is no other mechanism for clearing outdated data
        await db.KeyExpireAsync($"{AllExtProvidersKey}:{schoolYear}", exp, ExpireWhen.HasNoExpiry);
        await db.KeyExpireAsync($"{AllScheduleExtProvidersKey}:{schoolYear}", exp, ExpireWhen.HasNoExpiry);
    }

    private IDatabase GetDatabase()
    {
        return this.connectionMultiplexerAccessor.ConnectionMultiplexer.GetDatabase();
    }
}

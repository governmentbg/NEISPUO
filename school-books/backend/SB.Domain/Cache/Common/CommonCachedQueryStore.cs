namespace SB.Domain;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;

internal class CommonCachedQueryStore : ICommonCachedQueryStore
{
    private readonly IFusionCache fusionCache;
    private readonly ICommonCQSQueryRepository commonCQSQueryRepository;
    private readonly ICommonRedisRepository commonRedisRepository;
    private readonly DomainOptions domainOptions;
    private readonly ILogger<CommonCachedQueryStore> logger;
    private const string CommonCachedQueryStoreKeyPrefix = "CCQS";

    public CommonCachedQueryStore(
        IFusionCache fusionCache,
        ICommonCQSQueryRepository commonCQSQueryRepository,
        ICommonRedisRepository commonRedisRepository,
        IOptions<DomainOptions> domainOptions,
        ILogger<CommonCachedQueryStore> logger)
    {
        this.fusionCache = fusionCache;
        this.commonCQSQueryRepository = commonCQSQueryRepository;
        this.commonRedisRepository = commonRedisRepository;
        this.domainOptions = domainOptions.Value;
        this.logger = logger;
    }

    public async Task<bool> GetInstHasCBExtProviderAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        var cacheResult = await this.commonRedisRepository.GetInstHasCBExtProviderAsync(schoolYear, instId, ct);

        if (cacheResult.HasValue)
        {
            return cacheResult.Value;
        }

        // refresh the cache
        var result = await this.commonCQSQueryRepository.GetAllExtProvidersAsync(schoolYear, ct);
        await this.commonRedisRepository.CacheAllExtProvidersAsync(
            schoolYear,
            result,
            this.domainOptions.ShortCacheExpiration,
            ct);

        return result.Any(i => i.InstId == instId && i.ExtSystemId != null);
    }

    public async Task<bool> GetExtSystemIsInstCBExtProviderAsync(
        int extSystemId,
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        var cacheResult =
            await this.commonRedisRepository.GetExtSystemIsInstCBExtProviderAsync(
                extSystemId,
                schoolYear,
                instId,
                ct);

        if (cacheResult == true)
        {
            // use the cache only for positive results,
            // if not cached or negative, we need to check the database
            return true;
        }

        this.logger.LogWarning("CBExtProvider cache miss for extSystemId {extSystemId}, schoolYear {schoolYear}, instId {instId}", extSystemId, schoolYear, instId);

        // refresh the cache
        var result = await this.commonCQSQueryRepository.GetAllExtProvidersAsync(schoolYear, ct);
        await this.commonRedisRepository.CacheAllExtProvidersAsync(
            schoolYear,
            result,
            this.domainOptions.ShortCacheExpiration,
            ct);

        return result.Any(i =>
            i.ExtSystemId == extSystemId &&
            i.InstId == instId);
    }

    private static string ShouldSendNotificationKey(int personId, StudentSettingsNotificationType notificationType)
        => $"{CommonCachedQueryStoreKeyPrefix}:SSN:{personId}:{(int)notificationType}";

    public async Task<bool> ShouldSendNotificationAsync(int personId, StudentSettingsNotificationType notificationType, CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<bool>(
            key: ShouldSendNotificationKey(personId, notificationType),
            factory: async (ctx, ct) =>
            {
                bool result =
                    await this.commonCQSQueryRepository.ShouldSendNotificationAsync(
                        personId,
                        notificationType,
                        ct);

                // cache negatives for a short period
                ctx.Options.Duration =
                    result
                    ? this.domainOptions.MediumCacheExpiration
                    : this.domainOptions.ShortCacheExpiration;

                return result;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize,
                },
            token: ct);
    }

    public async Task<bool> GetExtSystemIsInstScheduleExtProviderAsync(
        int extSystemId,
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        var cacheResult =
            await this.commonRedisRepository.GetExtSystemIsInstScheduleExtProviderAsync(
                extSystemId,
                schoolYear,
                instId,
                ct);

        if (cacheResult == true)
        {
            // use the cache only for positive results,
            // if not cached or negative, we need to check the database
            return true;
        }

        this.logger.LogWarning("ScheduleExtProvider cache miss for extSystemId {extSystemId}, schoolYear {schoolYear}, instId {instId}", extSystemId, schoolYear, instId);

        // refresh the cache
        var result = await this.commonCQSQueryRepository.GetAllExtProvidersAsync(schoolYear, ct);
        await this.commonRedisRepository.CacheAllExtProvidersAsync(
            schoolYear,
            result,
            this.domainOptions.ShortCacheExpiration,
            ct);

        return result.Any(i =>
            i.ScheduleExtSystemId == extSystemId &&
            i.InstId == instId);
    }

    private static string GetInstitutionSchoolYearIsFinalizedKey(int schoolYear, int instId)
        => $"{CommonCachedQueryStoreKeyPrefix}:GISYIF:{schoolYear}:{instId}";
    public async Task<bool> GetSchoolYearIsFinalizedAsync(int schoolYear, int instId, CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<bool>(
            key: GetInstitutionSchoolYearIsFinalizedKey(schoolYear, instId),
            factory: async (ctx, ct) =>
            {
                bool isFinalized =
                    await this.commonCQSQueryRepository.GetInstitutionSchoolYearIsFinalizedAsync(
                        schoolYear,
                        instId,
                        ct);

                // cache negatives for a short period
                ctx.Options.Duration =
                    isFinalized
                    ? this.domainOptions.MediumCacheExpiration
                    : this.domainOptions.ShortCacheExpiration;

                return isFinalized;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize,
                },
            token: ct);
    }
}

namespace SB.Domain;

using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using ZiggyCreatures.Caching.Fusion;
using static SB.Domain.IExtApiAuthQueryRepository;

internal class ExtApiAuthCachedQueryStore : IExtApiAuthCachedQueryStore
{
    private readonly IFusionCache fusionCache;
    private readonly IExtApiAuthQueryRepository extApiAuthQueryRepository;
    private readonly DomainOptions domainOptions;
    private const string ExtApiAuthCacheKeyPrefix = "EAA";

    public ExtApiAuthCachedQueryStore(
        IFusionCache fusionCache,
        IExtApiAuthQueryRepository extApiAuthQueryRepository,
        IOptions<DomainOptions> domainOptions)
    {
        this.fusionCache = fusionCache;
        this.extApiAuthQueryRepository = extApiAuthQueryRepository;
        this.domainOptions = domainOptions.Value;
    }

    public async Task<GetExtSystemVO?> GetExtSystemAsync(string thumbprint, CancellationToken ct)
    {
        return await this.fusionCache.GetOrSetAsync<GetExtSystemVO?>(
            key: $"{ExtApiAuthCacheKeyPrefix}:{thumbprint}",
            factory: async (ctx, ct) =>
            {
                var res = await this.extApiAuthQueryRepository.GetExtSystemAsync(thumbprint, ct);
                if (res == null)
                {
                    // cache nulls for a short period
                    ctx.Options.Duration = this.domainOptions.ShortCacheExpiration;
                    return null;
                }

                ctx.Options.Duration = this.domainOptions.LongCacheExpiration;
                return res;
            },
            options:
                new FusionCacheEntryOptions
                {
                    Size = CacheConstants.SmallObjectSize
                },
            token: ct);
    }
}

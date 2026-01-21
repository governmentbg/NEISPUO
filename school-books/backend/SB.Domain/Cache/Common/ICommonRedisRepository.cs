namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using static SB.Domain.ICommonCQSQueryRepository;

public interface ICommonRedisRepository
{
    Task<bool?> GetInstHasCBExtProviderAsync(int schoolYear, int instId, CancellationToken ct);

    Task<bool?> GetExtSystemIsInstCBExtProviderAsync(int extSystemId, int schoolYear, int instId, CancellationToken ct);

    Task<bool?> GetExtSystemIsInstScheduleExtProviderAsync(int extSystemId, int schoolYear, int instId, CancellationToken ct);

    Task CacheAllExtProvidersAsync(int schoolYear, GetAllExtProvidersVO[] extProviders, TimeSpan exp, CancellationToken ct);
}

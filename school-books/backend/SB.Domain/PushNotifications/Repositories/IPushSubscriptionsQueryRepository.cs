namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IPushSubscriptionsQueryRepository : IRepository
{
    Task<GetAllByUsersVO[]> GetAllBySysUserIdsAsync(
        int[] sysUSerIds,
        CancellationToken ct);
}

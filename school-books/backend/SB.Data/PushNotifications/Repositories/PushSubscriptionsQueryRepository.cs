namespace SB.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Common;
using SB.Domain;
using static SB.Domain.IPushSubscriptionsQueryRepository;

internal class PushSubscriptionsQueryRepository : Repository, IPushSubscriptionsQueryRepository
{
    public PushSubscriptionsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllByUsersVO[]> GetAllBySysUserIdsAsync(
        int[] sysUserIds,
        CancellationToken ct)
    {
        return await (
            from ps in this.DbContext.Set<UserPushSubscription>()
            where this.DbContext.MakeIdsQuery(sysUserIds).Any(id => id.Id == id.Id)
            select new GetAllByUsersVO(
                ps.Endpoint,
                ps.P256dh,
                ps.Auth)
            ).Distinct()
       .ToArrayAsync(ct);
    }
}

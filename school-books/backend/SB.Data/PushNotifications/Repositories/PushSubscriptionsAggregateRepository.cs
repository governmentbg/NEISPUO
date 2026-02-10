namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class PushSubscriptionsAggregateRepository : BaseAggregateRepository<UserPushSubscription>, IPushSubscriptionsAggregateRepository
{
    public PushSubscriptionsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<UserPushSubscription?> FindOrDefaultAsync(
        string endPoint,
        CancellationToken ct)
    {
        return await (
            from ps in this.DbContext.Set<UserPushSubscription>()
            where ps.Endpoint == endPoint
            select ps
        ).SingleOrDefaultAsync(ct);
    }

    public async Task<UserPushSubscription[]> FindAllBySysUserIdAsync(
        int sysUserId,
        CancellationToken ct)
    {
        return await this.FindEntitiesAsync(a => a.SysUserId == sysUserId, ct);
    }
}

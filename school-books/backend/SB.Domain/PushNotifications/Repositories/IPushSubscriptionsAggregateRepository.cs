namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IPushSubscriptionsAggregateRepository : IRepository
{
    Task<UserPushSubscription?> FindOrDefaultAsync(string endpoint, CancellationToken ct);

    Task<UserPushSubscription[]> FindAllBySysUserIdAsync(int sysUserId, CancellationToken ct);

    Task AddAsync(UserPushSubscription entity, bool preventDetectChanges = false, CancellationToken ct = default);

    void Remove(UserPushSubscription entity);

    void Remove(UserPushSubscription entity, bool forceDetectChangesBeforeRemove = false, bool preventDetectChanges = false);
}

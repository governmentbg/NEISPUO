namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IScopedAggregateRepository<TEntity> : IRepository
    where TEntity : class, IAggregateRoot
{
    Task<TEntity> FindAsync(int schoolYear, int id, CancellationToken ct);

    Task AddAsync(TEntity entity, CancellationToken ct);

    Task AddAsync(TEntity entity, bool preventDetectChanges = false, CancellationToken ct = default);

    void Remove(TEntity entity);

    void Remove(TEntity entity, bool forceDetectChangesBeforeRemove = false, bool preventDetectChanges = false);
}

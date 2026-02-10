namespace SB.Data;

using SB.Domain;
using System.Threading;
using System.Threading.Tasks;

internal class ScopedAggregateRepository<TEntity> : BaseAggregateRepository<TEntity>, IScopedAggregateRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
    public ScopedAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public virtual Task<TEntity> FindAsync(int schoolYear, int id, CancellationToken ct)
    {
        return this.FindEntityAsync(
            this.DbContext.Set<TEntity>(),
            new object[] { schoolYear, id },
            this.Includes,
            ct);
    }
}

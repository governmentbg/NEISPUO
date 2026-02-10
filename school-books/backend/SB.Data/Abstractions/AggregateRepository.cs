namespace SB.Data;

using SB.Domain;
using System.Threading;
using System.Threading.Tasks;

internal class AggregateRepository<TEntity> : BaseAggregateRepository<TEntity>, IAggregateRepository<TEntity>
    where TEntity : class, IAggregateRoot
{
    public AggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public Task<TEntity> FindAsync(int id, CancellationToken ct)
    {
        return this.FindEntityAsync(
            this.DbContext.Set<TEntity>(),
            new object[] { id },
            this.Includes,
            ct);
    }

    public virtual async Task<TEntity> FindForUpdateAsync(int id, byte[] version, CancellationToken ct)
    {
        var entity = await this.FindAsync(id, ct);

        this.CheckVersion(entity.Version, version);

        return entity;
    }
}

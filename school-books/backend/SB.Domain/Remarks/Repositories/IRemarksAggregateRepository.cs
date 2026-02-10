namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IRemarksAggregateRepository : IScopedAggregateRepository<Remark>
{
    Task<Remark[]> FindAllByIdsAsync(
        int schoolYears,
        int[] remarkIds,
        CancellationToken ct);
}

namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IGradesAggregateRepository : IScopedAggregateRepository<Grade>
{
    Task<Grade[]> FindAllByIdsAsync(
        int schoolYears,
        int[] gradeIds,
        CancellationToken ct);
}

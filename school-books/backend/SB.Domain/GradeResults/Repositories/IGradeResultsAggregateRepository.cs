namespace SB.Domain;
using System.Threading;
using System.Threading.Tasks;

public interface IGradeResultsAggregateRepository : IScopedAggregateRepository<GradeResult>
{
    Task<GradeResult[]> FindAllByClassBookIdAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);
}

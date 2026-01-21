namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IFirstGradeResultsAggregateRepository : IScopedAggregateRepository<FirstGradeResult>
{
    Task<FirstGradeResult[]> FindAllByClassBookAsync(int schoolYear, int classBookId, CancellationToken ct);
}

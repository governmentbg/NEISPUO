namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface ITopicsAggregateRepository : IScopedAggregateRepository<Topic>
{
    Task<Topic[]> FindUsedTopicsAsync(
        int schoolYear,
        int classBookId,
        int[] classBookTopicPlanItemIds,
        CancellationToken ct);
}

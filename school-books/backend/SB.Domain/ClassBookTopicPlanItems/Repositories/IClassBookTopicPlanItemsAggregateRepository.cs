namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookTopicPlanItemsAggregateRepository : IScopedAggregateRepository<ClassBookTopicPlanItem>
{
    Task<ClassBookTopicPlanItem[]> FindAllByCurriculumIdAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    Task<ClassBookTopicPlanItem[]> FindAllByIdsAsync(
        int schoolYear,
        int classBookId,
        int[] classBookTopicPlanItemIds,
        CancellationToken ct);
}

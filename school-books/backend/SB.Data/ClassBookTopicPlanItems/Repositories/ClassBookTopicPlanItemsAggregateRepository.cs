namespace SB.Data;

using SB.Domain;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using SB.Common;

internal class ClassBookTopicPlanItemsAggregateRepository : ScopedAggregateRepository<ClassBookTopicPlanItem>, IClassBookTopicPlanItemsAggregateRepository
{
    public ClassBookTopicPlanItemsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<ClassBookTopicPlanItem[]> FindAllByCurriculumIdAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            tpi =>
                tpi.SchoolYear == schoolYear && tpi.ClassBookId == classBookId && tpi.CurriculumId == curriculumId,
            ct)
            ).ToArray();
    }

    public async Task<ClassBookTopicPlanItem[]> FindAllByIdsAsync(
        int schoolYear,
        int classBookId,
        int[] classBookTopicPlanItemIds,
        CancellationToken ct)
    {
        return (await this.FindEntitiesAsync(
            tpi =>
                tpi.SchoolYear == schoolYear
                && tpi.ClassBookId == classBookId
                && this.DbContext.MakeIdsQuery(classBookTopicPlanItemIds)
                    .Any(id => tpi.ClassBookTopicPlanItemId == id.Id),
            ct)
            ).ToArray();
    }
}

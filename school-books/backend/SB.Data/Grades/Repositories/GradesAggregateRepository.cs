namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SB.Common;
using SB.Domain;

internal class GradesAggregateRepository : ScopedAggregateRepository<Grade>, IGradesAggregateRepository
{
    public GradesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<Grade[]> FindAllByIdsAsync(
        int schoolYear,
        int[] gradeIds,
        CancellationToken ct)
    {
        return await this.FindEntitiesAsync(
            a => a.SchoolYear == schoolYear && this.DbContext.MakeIdsQuery(gradeIds).Any(id => a.GradeId == id.Id),
            ct);
    }
}

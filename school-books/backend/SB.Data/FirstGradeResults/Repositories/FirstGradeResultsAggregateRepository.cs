namespace SB.Data;

using SB.Domain;
using System.Threading;
using System.Threading.Tasks;

internal class FirstGradeResultsAggregateRepository : ScopedAggregateRepository<FirstGradeResult>, IFirstGradeResultsAggregateRepository
{
    public FirstGradeResultsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<FirstGradeResult[]> FindAllByClassBookAsync(int schoolYear, int classBookId, CancellationToken ct)
    {
        return await this.FindEntitiesAsync(
            e => e.SchoolYear == schoolYear &&
                e.ClassBookId == classBookId,
            ct);
    }
}

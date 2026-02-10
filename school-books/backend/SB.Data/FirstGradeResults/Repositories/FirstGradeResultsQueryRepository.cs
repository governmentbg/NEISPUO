namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IFirstGradeResultsQueryRepository;

internal class FirstGradeResultsQueryRepository : ScopedAggregateRepository<FirstGradeResult>, IFirstGradeResultsQueryRepository
{
    public FirstGradeResultsQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllVO[]> GetAllAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await (
            from r in this.DbContext.Set<FirstGradeResult>()

            where r.SchoolYear == schoolYear &&
                r.ClassBookId == classBookId

            select new GetAllVO(
                r.PersonId,
                r.QualitativeGrade,
                r.SpecialGrade))
            .ToArrayAsync(ct);
    }
}

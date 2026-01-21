namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;
using static SB.Domain.IAdditionalActivitiesQueryRepository;

internal class AdditionalActivitiesQueryRepository : Repository, IAdditionalActivitiesQueryRepository
{
    public AdditionalActivitiesQueryRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<GetAllAdditionalActivitiesForWeekVO[]> GetAllAdditionalActivitiesForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct)
    {
        return await (
            from t in this.DbContext.Set<AdditionalActivity>()

            where t.SchoolYear == schoolYear &&
                t.ClassBookId == classBookId &&
                t.Year == year &&
                t.WeekNumber == weekNumber

            select new GetAllAdditionalActivitiesForWeekVO(
                t.AdditionalActivityId,
                t.Activity,
                t.CreatedBySysUserId)
        ).ToArrayAsync(ct);
    }

    public async Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int additionalActivityId,
        CancellationToken ct)
    {
        return await this.DbContext.Set<AdditionalActivity>()
            .Where(aa =>
                aa.SchoolYear == schoolYear &&
                aa.ClassBookId == classBookId &&
                aa.AdditionalActivityId == additionalActivityId)
            .Select(aa => new GetVO(
                aa.AdditionalActivityId,
                aa.Activity,
                aa.CreatedBySysUserId
            ))
            .SingleAsync(ct);
    }
}

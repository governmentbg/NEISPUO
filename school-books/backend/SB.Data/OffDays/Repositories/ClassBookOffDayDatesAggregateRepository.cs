namespace SB.Data;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

internal class ClassBookOffDayDatesAggregateRepository : BaseAggregateRepository<ClassBookOffDayDate>, IClassBookOffDayDatesAggregateRepository
{
    public ClassBookOffDayDatesAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    public async Task<ClassBookOffDayDate[]> FindAllByInstitutionAsync(
        int schoolYear,
        int instId,
        CancellationToken ct)
    {
        return await (
            from cbod in this.DbContext.Set<ClassBookOffDayDate>()

            join od in this.DbContext.Set<OffDay>()
            on new { cbod.SchoolYear, cbod.OffDayId }
            equals new { od.SchoolYear, od.OffDayId }

            where od.SchoolYear == schoolYear &&
                od.InstId == instId

            select cbod
        ).ToArrayAsync(ct);
    }

    public async Task<ClassBookOffDayDate[]> FindAllByClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct)
    {
        return await (
            from cbod in this.DbContext.Set<ClassBookOffDayDate>()

            where cbod.SchoolYear == schoolYear &&
                cbod.ClassBookId == classBookId

            select cbod
        ).ToArrayAsync(ct);
    }
}

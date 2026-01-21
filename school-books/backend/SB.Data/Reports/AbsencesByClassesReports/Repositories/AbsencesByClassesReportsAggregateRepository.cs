namespace SB.Data;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SB.Domain;

class AbsencesByClassesReportsAggregateRepository : ScopedAggregateRepository<AbsencesByClassesReport>, IAbsencesByClassesReportsAggregateRepository
{
    public AbsencesByClassesReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<AbsencesByClassesReport>, IQueryable<AbsencesByClassesReport>>[] Includes =>
        new Func<IQueryable<AbsencesByClassesReport>, IQueryable<AbsencesByClassesReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int absencesByClassesId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<AbsencesByClassesReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.AbsencesByClassesReportId == absencesByClassesId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<AbsencesByClassesReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.AbsencesByClassesReportId == absencesByClassesId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}

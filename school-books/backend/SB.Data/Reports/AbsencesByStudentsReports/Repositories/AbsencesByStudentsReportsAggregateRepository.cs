namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class AbsencesByStudentsReportsAggregateRepository : ScopedAggregateRepository<AbsencesByStudentsReport>, IAbsencesByStudentsReportsAggregateRepository
{
    public AbsencesByStudentsReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<AbsencesByStudentsReport>, IQueryable<AbsencesByStudentsReport>>[] Includes =>
        new Func<IQueryable<AbsencesByStudentsReport>, IQueryable<AbsencesByStudentsReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int absencesByStudentsId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<AbsencesByStudentsReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.AbsencesByStudentsReportId == absencesByStudentsId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<AbsencesByStudentsReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.AbsencesByStudentsReportId == absencesByStudentsId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}

namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class DateAbsencesReportsAggregateRepository : ScopedAggregateRepository<DateAbsencesReport>, IDateAbsencesReportsAggregateRepository
{
    public DateAbsencesReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<DateAbsencesReport>, IQueryable<DateAbsencesReport>>[] Includes =>
        new Func<IQueryable<DateAbsencesReport>, IQueryable<DateAbsencesReport>>[]
        {
            (q) => q.Include(e => e.Items)
        };

    public async Task RemoveAsync(int schoolYear, int dateAbsencesId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<DateAbsencesReportItem>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.DateAbsencesReportId == dateAbsencesId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<DateAbsencesReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.DateAbsencesReportId == dateAbsencesId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}

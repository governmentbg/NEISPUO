namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ScheduleAndAbsencesByMonthReportsAggregateRepository : ScopedAggregateRepository<ScheduleAndAbsencesByMonthReport>, IScheduleAndAbsencesByMonthReportsAggregateRepository
{
    public ScheduleAndAbsencesByMonthReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<ScheduleAndAbsencesByMonthReport>, IQueryable<ScheduleAndAbsencesByMonthReport>>[] Includes =>
        new Func<IQueryable<ScheduleAndAbsencesByMonthReport>, IQueryable<ScheduleAndAbsencesByMonthReport>>[]
        {
            (q) => q.Include(e => e.Weeks).ThenInclude(w => w.Days).ThenInclude(d => d.Hours),
        };

    public async Task RemoveAsync(int schoolYear, int scheduleAndAbsencesByMonthReportId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByMonthReportWeekDayHour>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByMonthReportId == scheduleAndAbsencesByMonthReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByMonthReportWeekDay>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByMonthReportId == scheduleAndAbsencesByMonthReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByMonthReportWeek>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByMonthReportId == scheduleAndAbsencesByMonthReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByMonthReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByMonthReportId == scheduleAndAbsencesByMonthReportId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}

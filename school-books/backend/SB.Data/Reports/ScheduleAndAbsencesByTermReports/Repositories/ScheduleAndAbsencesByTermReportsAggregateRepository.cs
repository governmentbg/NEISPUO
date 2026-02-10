namespace SB.Data;

using Microsoft.EntityFrameworkCore;
using SB.Domain;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

internal class ScheduleAndAbsencesByTermReportsAggregateRepository : ScopedAggregateRepository<ScheduleAndAbsencesByTermReport>, IScheduleAndAbsencesByTermReportsAggregateRepository
{
    public ScheduleAndAbsencesByTermReportsAggregateRepository(UnitOfWork unitOfWork)
        : base(unitOfWork)
    {
    }

    protected override Func<IQueryable<ScheduleAndAbsencesByTermReport>, IQueryable<ScheduleAndAbsencesByTermReport>>[] Includes =>
        new Func<IQueryable<ScheduleAndAbsencesByTermReport>, IQueryable<ScheduleAndAbsencesByTermReport>>[]
        {
            (q) => q.Include(e => e.Weeks).ThenInclude(w => w.Days).ThenInclude(d => d.Hours),
        };

    public async Task RemoveAsync(int schoolYear, int scheduleAndAbsencesByTermReportId, CancellationToken ct)
    {
        await using var transaction = await this.DbContext.Database.BeginTransactionAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByTermReportWeekDayHour>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByTermReportWeekDay>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByTermReportWeek>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId)
            .ExecuteDeleteAsync(ct);

        await this.DbContext.Set<ScheduleAndAbsencesByTermReport>()
            .Where(e =>
                e.SchoolYear == schoolYear &&
                e.ScheduleAndAbsencesByTermReportId == scheduleAndAbsencesByTermReportId)
            .ExecuteDeleteAsync(ct);

        await transaction.CommitAsync(ct);
    }
}

namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IScheduleAndAbsencesByMonthReportsAggregateRepository : IScopedAggregateRepository<ScheduleAndAbsencesByMonthReport>
{
    Task RemoveAsync(
        int schoolYear,
        int scheduleAndAbsencesByMonthReportId,
        CancellationToken ct);
}

namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IScheduleAndAbsencesByTermReportsAggregateRepository : IScopedAggregateRepository<ScheduleAndAbsencesByTermReport>
{
    Task RemoveAsync(
        int schoolYear,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct);
}

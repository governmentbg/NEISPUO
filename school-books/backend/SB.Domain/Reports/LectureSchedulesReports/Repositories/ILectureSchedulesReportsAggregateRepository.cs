namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface ILectureSchedulesReportsAggregateRepository : IScopedAggregateRepository<LectureSchedulesReport>
{
    Task RemoveAsync(
        int schoolYear,
        int lectureSchedulesReportId,
        CancellationToken ct);
}

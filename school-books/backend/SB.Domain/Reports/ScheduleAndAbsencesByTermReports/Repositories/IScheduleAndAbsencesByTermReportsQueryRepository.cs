namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IScheduleAndAbsencesByTermReportsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct);

    Task<GetWeeksVO[]> GetWeeksAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct);

    Task<GetWeeksForAddVO[]> GetWeeksForAddAsync(
        int schoolYear,
        int instId,
        SchoolTerm term,
        int classBookId,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByTermReportId,
        CancellationToken ct);
}

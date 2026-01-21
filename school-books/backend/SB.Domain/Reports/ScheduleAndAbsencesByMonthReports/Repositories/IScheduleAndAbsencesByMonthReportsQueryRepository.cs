namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IScheduleAndAbsencesByMonthReportsQueryRepository
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
        int scheduleAndAbsencesByMonthReportId,
        CancellationToken ct);

    Task<GetWeeksVO[]> GetWeeksAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByMonthReportId,
        CancellationToken ct);

    Task<GetWeeksForAddVO[]> GetWeeksForAddAsync(
        int schoolYear,
        int year,
        int month,
        int classBookId,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int scheduleAndAbsencesByMonthReportId,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByMonthReportId,
        CancellationToken ct);
}

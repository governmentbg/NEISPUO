namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using SB.Common;

public partial interface IFinalGradePointAverageByClassesReportsQueryRepository
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
        int finalGradePointAverageByClassesReportId,
        CancellationToken ct);

    Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByClassesReportId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        ReportPeriod period,
        int[] classBooks,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int finalGradePointAverageByClassesReportId,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByClassesReportId,
        CancellationToken ct);
}

namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using SB.Common;

public partial interface IFinalGradePointAverageByStudentsReportsQueryRepository
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
        int finalGradePointAverageByStudentsReportId,
        CancellationToken ct);

    Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByStudentsReportId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        ReportPeriod period,
        int[] classBooks,
        decimal? minimalGradePointAverage,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int finalGradePointAverageByStudentsReportId,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByStudentsReportId,
        CancellationToken ct);
}

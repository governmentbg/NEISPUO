namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IGradelessStudentsReportsQueryRepository
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
        int gradelessStudentsReportId,
        CancellationToken ct);

    Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        bool OnlyFinalGrades,
        ReportPeriod period,
        int[] ClassBooks,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        CancellationToken ct);
}

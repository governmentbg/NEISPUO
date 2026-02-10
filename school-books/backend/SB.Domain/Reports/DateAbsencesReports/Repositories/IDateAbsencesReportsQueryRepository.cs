namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IDateAbsencesReportsQueryRepository
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
        int dateAbsencesReportId,
        CancellationToken ct);

    Task<GetItemsVO[]> GetItemsAsync(
        int schoolYear,
        int dateAbsencesReportId,
        CancellationToken ct);

    Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        DateTime reportDate,
        bool isUnited,
        int[] classBooks,
        int[] shifts,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int dateAbsencesReportId,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int dateAbsencesReportId,
        CancellationToken ct);
}

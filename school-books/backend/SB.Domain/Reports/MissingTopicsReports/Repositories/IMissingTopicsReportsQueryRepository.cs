namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IMissingTopicsReportsQueryRepository
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
        int missingTopicsReportId,
        CancellationToken ct);

    Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        MissingTopicsReportPeriod period,
        int? year,
        int? month,
        int? teacherPersonId,
        DateTime createDate,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        CancellationToken ct);
}

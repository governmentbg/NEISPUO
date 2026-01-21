namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;
using SB.Common;

public partial interface IStudentsAtRiskOfDroppingOutReportsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        int sysUserId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetItemsForAddVO[]> GetItemsForAddAsync(
        int schoolYear,
        int instId,
        DateTime reportDate,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        CancellationToken ct);

    Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<int> GetCreatedBySysUserIdAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        CancellationToken ct);

    Task<GetExcelDataVO> GetExcelDataAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        CancellationToken ct);
}

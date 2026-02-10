namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface IPerformancesQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int performanceId,
        CancellationToken ct);

    Task<GetExcelDataVO[]> GetExcelDataAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GetExcelDataVO[]> GetAllExcelDataAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);
}

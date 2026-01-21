namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface ISupportsQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int classBookId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int supportId,
        CancellationToken ct);

    Task<TableResultVO<GetActivityAllVO>> GetActivityAllAsync(
        int schoolYear,
        int supportId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetActivityVO> GetActivityAsync(
        int schoolYear,
        int supportId,
        int supporActivitytId,
        CancellationToken ct);
}

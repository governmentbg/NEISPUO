namespace SB.Domain;

using SB.Common;
using System.Threading;
using System.Threading.Tasks;

public partial interface ITopicPlansQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int sysUserId,
        string? name,
        string? basicClassName,
        string? subjectName,
        string? subjectTypeName,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int sysUserId,
        int topicPlanId,
        CancellationToken ct);

    Task<TableResultVO<GetItemsVO>> GetItemsAsync(
        int sysUserId,
        int topicPlanId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetItemVO> GetItemAsync(
        int sysUserId,
        int topicPlanItemId,
        CancellationToken ct);

    Task<GetExcelDataVO[]> GetExcelDataAsync(
        int sysUserId,
        int topicPlanId,
        CancellationToken ct);

    Task RemoveTopicPlanAsync(
        int topicPlanId,
        CancellationToken ct);
}

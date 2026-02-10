namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassGroupsQueryRepository
{
    Task<GetAllForClassKindVO[]> GetAllForClassKindAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        CancellationToken ct);

    Task<GetAllForClassKindVO[]> GetAllForReorganizeAsync(
        int schoolYear,
        int instId,
        ClassKind classKind,
        ClassBookReorganizeType reorganizeType,
        CancellationToken ct);

    Task<GetExtApiClassGroupsVO[]> GetClassGroupsAsync(
        int schoolYear,
        int instId,
        int[] classIds,
        CancellationToken ct);

    Task<GetClassGroupsWIthClassBookVO[]> GetClassGroupsWithClassBookAsync(
        int schoolYear,
        int instId,
        int[] classIds,
        CancellationToken ct);

    Task<GetInvalidClassGroupsForClassBookCreationVO[]> GetInvalidClassGroupsForClassBookCreationAsync(
        int schoolYear,
        int instId,
        int[] classIds,
        CancellationToken ct);

    Task<InstType> GetInstTypeAsync(
        int schoolYear,
        int instId,
        CancellationToken ct);
}

namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public interface ITopicPlanNomsRepository
{
    Task<NomVO[]> GetNomsByIdAsync(
        int personId,
        int[] ids,
        CancellationToken ct);

    Task<NomVO[]> GetNomsByTermAsync(
        int personId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct);
}

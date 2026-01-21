namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public interface INomsRepository
{
    Task<NomVO[]> GetNomsByIdAsync(int[] ids, CancellationToken ct);

    Task<NomVO[]> GetNomsByTermAsync(string? term, int? offset, int? limit, CancellationToken ct);
}

namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public interface IScopedNomsRepository
{
    Task<NomVO[]> GetNomsByIdAsync(int schoolYear, int instId, int[] ids, CancellationToken ct);

    Task<NomVO[]> GetNomsByTermAsync(int schoolYear, int instId, string? term, int? offset, int? limit, CancellationToken ct);
}

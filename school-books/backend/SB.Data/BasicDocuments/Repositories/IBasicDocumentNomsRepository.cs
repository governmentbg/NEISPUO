namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public interface IBasicDocumentNomsRepository
{
    Task<NomVO[]> GetNomsByIdAsync(int[] ids, RegBookType type, CancellationToken ct);

    Task<NomVO[]> GetNomsByTermAsync(string? term, RegBookType type, int? offset, int? limit, CancellationToken ct);
}

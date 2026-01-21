namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookNomsRepository
{
    Task<NomVO[]> GetNomsByIdAsync(int schoolYear, int instId, int[] ids, CancellationToken ct);

    Task<NomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        bool? showPG,
        bool? showCdo,
        bool? showDplr,
        bool? showCsop,
        bool? showInvalid,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct);
}

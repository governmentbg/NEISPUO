namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public partial interface IInstTeacherNomsRepository
{
    Task<InstTeacherNomVO[]> GetNomsByIdAsync(int instId, int[] ids, CancellationToken ct);

    Task<InstTeacherNomVO[]> GetNomsByTermAsync(
        int instId,
        int schoolYear,
        string? term,
        bool? includeNonPedagogical,
        bool? includeNotActiveTeachers,
        bool? includeNoReplacementTeachers,
        int? offset,
        int? limit,
        CancellationToken ct);
}

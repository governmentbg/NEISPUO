namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookSubjectNomsRepository
{
    Task<NomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int[] ids,
        CancellationToken ct);

    Task<NomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? writeAccessCurriculumTeacherPersonId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct);
}

namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public partial interface IInstitutionCurriculumNomsRepository
{
    Task<InstitutionCurriculumNomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        InstitutionCurriculumNomVOCurriculum[] ids,
        CancellationToken ct);

    Task<InstitutionCurriculumNomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct);
}

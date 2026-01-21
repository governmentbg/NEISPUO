namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public partial interface IInstitutionStudentNomsRepository
{
    Task<InstitutionStudentNomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        InstitutionStudentNomVOStudent[] ids,
        CancellationToken ct);

    Task<InstitutionStudentNomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        string? term,
        int? offset,
        int? limit,
        bool? showOnlyLastGrade,
        CancellationToken ct);
}

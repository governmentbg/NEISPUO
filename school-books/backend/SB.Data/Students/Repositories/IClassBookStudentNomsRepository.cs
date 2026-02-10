namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public partial interface IClassBookStudentNomsRepository
{
    Task<ClassBookStudentNomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        int classBookId,
        bool showOnlyWithIndividualCurriculum,
        bool showOnlyWithIndividualCurriculumSchedule,
        int[] ids,
        CancellationToken ct);

    Task<ClassBookStudentNomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        string? term,
        int classBookId,
        bool showOnlyWithIndividualCurriculum,
        bool showOnlyWithIndividualCurriculumSchedule,
        int? offset,
        int? limit,
        CancellationToken ct);
}

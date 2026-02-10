namespace SB.Data;

using System.Threading;
using System.Threading.Tasks;

public interface IClassBookCurriculumNomsRepository
{
    Task<NomVO[]> GetNomsByIdAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? individualCurriculumPersonId,
        int? gradeResultStudentPersonId,
        int[] ids,
        CancellationToken ct);

    Task<NomVO[]> GetNomsByTermAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int? curriculumTeacherPersonId,
        int? individualCurriculumPersonId,
        int? gradeResultStudentPersonId,
        bool? excludeIndividualCurriculums,
        string? term,
        int? offset,
        int? limit,
        CancellationToken ct);
}

namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IPgResultsQueryRepository
{
    Task<GetAllForClassBookVO[]> GetAllForClassBookAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GetAllForStudentVO[]> GetAllForStudentAsync(
        int schoolYear,
        int classBookId,
        int studentPersonId,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int pgResultId,
        CancellationToken ct);

    Task<bool> ExistsForPersonAndSubjectAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int? curriculumId,
        CancellationToken ct);

    Task<int> GetSubjectIdForCurriculumAsync(
        int curriculumId,
        CancellationToken ct);
}

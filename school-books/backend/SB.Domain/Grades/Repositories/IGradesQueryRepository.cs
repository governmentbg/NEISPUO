namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IGradesQueryRepository
{
    public Task<GetCurriculumStudentsVO[]> GetCurriculumStudentsAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    public Task<GetCurriculumGradesVO[]> GetCurriculumGradesAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    public Task<GetCurriculumVO> GetCurriculumAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    public Task<GetVO> GetAsync(
        int schoolYear,
        int classBookId,
        int gradeId,
        CancellationToken ct);

    public Task<GetProfilingSubjectForecastGradesVO[]> GetProfilingSubjectForecastGradesAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    public Task<bool> ExistsVerifiedScheduleLessonAsync(
        int schoolYear,
        int scheduleLessonId,
        CancellationToken ct);

    public Task<bool> ExistsVerifiedScheduleLessonForGradeAsync(
        int schoolYear,
        int gradeId,
        CancellationToken ct);

    public Task<bool> ExistsFinalGradeForStudentAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        CancellationToken ct);

    public Task<bool> ExistsTermGradeForStudentAsync(
        int schoolYear,
        int classBookId,
        int personId,
        int curriculumId,
        SchoolTerm term,
        CancellationToken ct);
}

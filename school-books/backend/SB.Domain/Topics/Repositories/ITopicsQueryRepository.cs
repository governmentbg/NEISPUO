namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface ITopicsQueryRepository
{
    Task<GetAllForWeekVO[]> GetAllForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct);

    Task<GetUndoInfoByIdsVO[]> GetUndoInfoByIdsAsync(
        int schoolYear,
        int[] topicIds,
        CancellationToken ct);

    Task<int[]> GetCurriculumsWithTopicPlanAsync(
        int schoolYear,
        int classBookId,
        CancellationToken ct);

    Task<GetCurriculumTopiPlanVO[]> GetCurriculumTopiPlanAsync(
        int schoolYear,
        int classBookId,
        int curriculumId,
        CancellationToken ct);

    Task<GetScheduleLessonsTeachersVO[]> GetScheduleLessonsTeachersAsync(
        int schoolYear,
        int[] scheduleLessonId,
        CancellationToken ct);

    public Task<bool> ExistsVerifiedScheduleLessonForTopicsAsync(
        int schoolYear,
        int[] topicIds,
        CancellationToken ct);

    public Task<bool> ExistsTopicStudentsInInstitution(
        int schoolYear,
        int instId,
        int[] studentPersonIds,
        CancellationToken ct);
}

namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface ITopicsDplrQueryRepository
{
    Task<GetAllDplrForWeekVO[]> GetAllForWeekAsync(
        int schoolYear,
        int classBookId,
        int year,
        int weekNumber,
        CancellationToken ct);

    Task<GetUndoInfoByIdsVO[]> GetTopicsDplrUndoInfoByIdAsync(
        int schoolYear,
        int topicDplrId,
        CancellationToken ct);

    Task<int[]> GetCurriculumTeacherIdsAsync(
        int schoolYear,
        int curriculumId,
        CancellationToken ct);

    public Task<bool> ExistsTopicStudentsInInstitution(
        int schoolYear,
        int instId,
        int[] studentPersonIds,
        CancellationToken ct);

    Task<bool> ExistsVerifiedTopicDplrAsync(
        int schoolYear,
        int topicDplrId,
        CancellationToken ct);

    Task<int[]> GetExistingHourNumbersForDateAsync(
        int schoolYear,
        int classBookId,
        DateTime date,
        CancellationToken ct);
}

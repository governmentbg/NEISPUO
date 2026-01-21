namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public interface ISchedulesAggregateRepository : IScopedAggregateRepository<Schedule>
{
    Task<Schedule[]> FindAllByClassBookAsync(int schoolYear, int classBookId, CancellationToken ct);

    Task<Schedule[]> FindSchedulesByDateAsync(int schoolYear, int classBookId, DateTime date, CancellationToken ct);

    Task<ScheduleLesson[]> FindScheduleLessonsAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        CancellationToken ct);

    Task RemoveScheduleLessonsForDateAsync(
        int schoolYear,
        int instId,
        int classBookId,
        DateTime date,
        CancellationToken ct);
}

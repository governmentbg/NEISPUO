namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public partial interface IBookVerificationQueryRepository
{
    Task<GetYearViewVO[]> GetYearViewAsync(
        int schoolYear,
        int instId,
        int? classBookId,
        int? teacherPersonId,
        CancellationToken ct);

    Task<GetMonthViewVO[]> GetMonthViewAsync(
        int schoolYear,
        int instId,
        int year,
        int month,
        int? classBookId,
        int? teacherPersonId,
        CancellationToken ct);

    Task<GetScheduleLessonsForDayVO[]> GetScheduleLessonsForDayAsync(
        int schoolYear,
        int instId,
        int year,
        int month,
        int day,
        int? classBookId,
        int? teacherPersonId,
        CancellationToken ct);

    Task<bool> ExistTopicsAsync(
        int schoolYear,
        int[] scheduleLessonIds,
        CancellationToken ct);

    Task<GetOffDayVO[]> GetOffDaysForDayAsync(
        int schoolYear,
        int instId,
        int year,
        int month,
        int day,
        int? classBookId,
        CancellationToken ct);
}

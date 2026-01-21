namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface ITeacherAbsencesQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        string? teacherName,
        string? replTeacherName,
        int? offset,
        int? limit,
        CancellationToken ct);


    Task<TableResultVO<GetAllVO>> GetAllForAbsenteePersonAsync(
        int schoolYear,
        int instId,
        int personId,
        string? replTeacherName,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<TableResultVO<GetAllVO>> GetAllForReplacementPersonAsync(
        int schoolYear,
        int instId,
        int personId,
        string? teacherName,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct);

    Task<bool> ContainsTeacherAsync(
        int schoolYear,
        int teacherAbsenceId,
        int personId,
        CancellationToken ct);

    Task<GetLessonsVO[]> GetLessonsAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        CancellationToken ct);

    Task<GetLessonsInUseVO[]> GetLessonsInUseAsync(
        int schoolYear,
        int instId,
        int[] scheduleLessonIds,
        int? exceptTeacherAbsenceId,
        CancellationToken ct);

    Task<GetTeacherAbsenceHoursInUseVO[]> GetTeacherAbsenceHoursInUseAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct);

    Task<GetScheduleVO> GetScheduleAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct);

    Task<GetScheduleVO> GetTeacherScheduleForPeriodAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct);

    Task<GetScheduleVO> GetTeacherScheduleForAbsenceAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct);

    Task<(int ClassBookId, DateTime Date)[]> GetOffDayDatesForClassBooksAsync(
        int schoolYear,
        int instId,
        DateTime from,
        DateTime to,
        int[] classBookIds,
        CancellationToken ct);

    Task<bool> HasInvalidClassBooksForTeacherAbsenceAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct);
}

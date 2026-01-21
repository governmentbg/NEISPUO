namespace SB.Domain;

using SB.Common;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface ILectureSchedulesQueryRepository
{
    Task<TableResultVO<GetAllVO>> GetAllAsync(
        int schoolYear,
        int instId,
        string? teacherName,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<TableResultVO<GetAllVO>> GetAllForTeacherPersonAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        int? offset,
        int? limit,
        CancellationToken ct);

    Task<GetVO> GetAsync(
        int schoolYear,
        int instId,
        int teacherAbsenceId,
        CancellationToken ct);

    Task<int> GetTeacherPersonIdAsync(
        int schoolYear,
        int teacherAbsenceId,
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
        int? exceptLectureScheduleId,
        CancellationToken ct);

    Task<GetScheduleVO> GetScheduleAsync(
        int schoolYear,
        int instId,
        int lectureScheduleId,
        CancellationToken ct);

    Task<GetScheduleVO> GetTeacherScheduleForPeriodAsync(
        int schoolYear,
        int instId,
        int teacherPersonId,
        DateTime startDate,
        DateTime endDate,
        CancellationToken ct);

    Task<GetScheduleVO> GetTeacherScheduleForLectureScheduleAsync(
        int schoolYear,
        int instId,
        int lectureScheduleId,
        CancellationToken ct);

    Task<bool> HasInvalidClassBooksForLectureScheduleAsync(
        int schoolYear,
        int instId,
        int lectureScheduleId,
        CancellationToken ct);
}

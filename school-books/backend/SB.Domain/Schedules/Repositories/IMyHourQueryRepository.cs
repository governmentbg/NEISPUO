namespace SB.Domain;

using System;
using System.Threading;
using System.Threading.Tasks;

public partial interface IMyHourQueryRepository
{
    Task<GetTeacherLessonsVO[]> GetTeacherLessonsAsync(
        int schoolYear,
        int instId,
        DateTime date,
        int teacherPersonId,
        CancellationToken ct);

    Task<GetTeacherLessonVO> GetTeacherLessonAsync(
        int schoolYear,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct);

    Task<GetTeacherLessonAbsencesVO[]> GetTeacherLessonAbsencesAsync(
        int schoolYear,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct);

    Task<GetTeacherLessonGradesVO[]> GetTeacherLessonGradesAsync(
        int schoolYear,
        int classBookId,
        int scheduleLessonId,
        CancellationToken ct);
}

namespace SB.Domain;

using System;

public partial interface ITeacherAbsencesQueryRepository
{
    public record GetLessonsInUseVO(
        int ScheduleLessonId,
        DateTime Date,
        int Day,
        int HourNumber,
        string ClassName);
}

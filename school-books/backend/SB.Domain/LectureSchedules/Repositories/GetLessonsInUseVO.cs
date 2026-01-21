namespace SB.Domain;

using System;

public partial interface ILectureSchedulesQueryRepository
{
    public record GetLessonsInUseVO(
        int ScheduleLessonId,
        DateTime Date,
        int Day,
        int HourNumber,
        string ClassName);
}

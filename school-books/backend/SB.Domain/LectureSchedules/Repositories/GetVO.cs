namespace SB.Domain;

using System;

public partial interface ILectureSchedulesQueryRepository
{
    public record GetVO(
        int TeacherPersonId,
        string OrderNumber,
        DateTime OrderDate,
        DateTime StartDate,
        DateTime EndDate,
        GetVOHour[] Hours);

    public record GetVOHour(
        int ScheduleLessonId,
        DateTime Date);
}

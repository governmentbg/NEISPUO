namespace SB.Domain;

using System;

public partial interface ILectureSchedulesQueryRepository
{
    public record GetAllVO(
        int LectureScheduleId,
        int TeacherPersonId,
        string TeacherName,
        string OrderNumber,
        DateTime OrderDate,
        DateTime StartDate,
        DateTime EndDate);
}

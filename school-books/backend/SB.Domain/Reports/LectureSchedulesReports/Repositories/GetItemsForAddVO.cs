namespace SB.Domain;

using System;

public partial interface ILectureSchedulesReportsQueryRepository
{
    public record GetItemsForAddVO(
        int TeacherPersonId,
        string TeacherPersonName,
        DateTime Date,
        int ClassBookId,
        string ClassBookName,
        int CurriculumId,
        string CurriculumName,
        int LectureScheduleId,
        string OrderNumber,
        DateTime OrderDate,
        int HoursTaken);
}

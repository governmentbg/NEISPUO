namespace SB.Domain;

using System;

public partial interface ILectureSchedulesReportsQueryRepository
{
    public record GetItemsVO(
        GetItemsVOInfoRow? InfoRow,
        GetItemsVOTotalRow? TotalRow);

    public record GetItemsVOInfoRow(
        int TeacherPersonId,
        string TeacherPersonName,
        DateTime Date,
        string ClassBookName,
        string CurriculumName,
        int? LectureScheduleId,
        string OrderNumber,
        DateTime OrderDate,
        int HoursTaken);

    public record GetItemsVOTotalRow(
        int TotalHoursTaken);
}

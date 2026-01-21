namespace SB.Domain;

using System;

public partial interface ILectureSchedulesReportsQueryRepository
{
    public record GetExcelDataVO(
        string Period,
        string? YearAndMonth,
        string? TeacherName,
        DateTime CreateDate,
        GetExcelDataVOTeacher[] Teachers);

    public record GetExcelDataVOTeacher(
        GetExcelDataVOTeacherHour[] Hours,
        int TotalHoursTaken);

    public record GetExcelDataVOTeacherHour(
        string TeacherName,
        DateTime Date,
        string ClassBookName,
        string CurriculumName,
        string OrderNumber,
        DateTime OrderDate,
        int HoursTaken);
}

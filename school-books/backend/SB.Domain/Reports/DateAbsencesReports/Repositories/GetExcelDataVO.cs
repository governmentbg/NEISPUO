namespace SB.Domain;

using System;

public partial interface IDateAbsencesReportsQueryRepository
{
    public record GetExcelDataVO(
        DateTime ReportDate,
        bool IsUnited,
        string? ClassBookNames,
        string? ShiftNames,
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
        int ClassBookId,
        string ClassBookName,
        bool IsOffDay,
        bool HasScheduleDate,
        GetExcelDataVOClassItemHour[] Hours);

    public record GetExcelDataVOClassItemHour(
        int? ShiftId,
        string? ShiftName,
        int HourNumber,
        string? AbsenceStudentNumbers,
        int AbsenceStudentCount);
}

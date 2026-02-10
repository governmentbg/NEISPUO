namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByTermReportsQueryRepository
{
    public record GetExcelDataVO(
        string Term,
        string ClassBookName,
        bool IsDPLR,
        DateTime CreateDate,
        GetExcelDataVOWeek[] Weeks);

    public record GetExcelDataVOWeek(
        string? StudentName,
        string WeekName,
        string? AdditionalActivities,
        GetExcelDataVOWeekDay[] Days);

    public record GetExcelDataVOWeekDay(
        DateTime Date,
        string DayName,
        bool IsOffDay,
        bool IsEmptyDay,
        GetExcelDataVOWeekDayHour[] Hours);

    public record GetExcelDataVOWeekDayHour(
        int HourNumber,
        bool? IsEmptyHour,
        string? CurriculumName,
        string? CurriculumTeacherNames,
        string? ExcusedStudentClassNumbers,
        string? UnexcusedStudentClassNumbers,
        string? LateStudentClassNumbers,
        string? DplrAbsenceStudentClassNumbers,
        string? DplrAttendanceStudentClassNumbers,
        string? Topics);
}

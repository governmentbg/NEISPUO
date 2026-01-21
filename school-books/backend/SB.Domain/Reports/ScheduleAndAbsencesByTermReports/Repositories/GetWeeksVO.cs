namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByTermReportsQueryRepository
{
    public record GetWeeksVO(
        string? StudentName,
        string WeekName,
        string? AdditionalActivities,
        GetWeeksVODay[] Days);

    public record GetWeeksVODay(
        DateTime Date,
        string DayName,
        bool IsOffDay,
        bool IsEmptyDay,
        GetWeekVODayHour[] Hours);

    public record GetWeekVODayHour(
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

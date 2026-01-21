namespace SB.Domain;

using System;

public partial interface IScheduleAndAbsencesByTermReportsQueryRepository
{
    public record GetWeeksForAddVO(
        string? StudentName,
        int Year,
        int WeekNumber,
        string WeekName,
        string? AdditionalActivities,
        GetWeeksForAddVODay[] Days);

    public record GetWeeksForAddVODay(
        DateTime Date,
        string DayName,
        bool IsOffDay,
        bool IsEmptyDay,
        GetWeeksForAddVODayHour[] Hours);

    public record GetWeeksForAddVODayHour(
        int HourNumber,
        string? CurriculumName,
        string? CurriculumTeacherNames,
        bool? IsEmptyHour,
        string? ExcusedStudentClassNumbers,
        string? UnexcusedStudentClassNumbers,
        string? LateStudentClassNumbers,
        string? DplrAbsenceStudentClassNumbers,
        string? DplrAttendanceStudentClassNumbers,
        string? Topics);
}

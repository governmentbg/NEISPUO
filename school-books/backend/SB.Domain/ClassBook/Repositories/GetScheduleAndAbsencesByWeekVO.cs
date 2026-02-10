namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetScheduleAndAbsencesByWeekVO(
        string? StudentName,
        int Year,
        int WeekNumber,
        string WeekName,
        string[] AdditionalActivities,
        GetScheduleAndAbsencesByWeekVODay[] Days);

    public record GetScheduleAndAbsencesByWeekVODay(
        int DayNumber,
        DateTime Date,
        string DayName,
        bool IsOffDay,
        bool IsEmptyDay,
        GetScheduleAndAbsencesByWeekVOHour[] Hours);

    public record GetScheduleAndAbsencesByWeekVOHour(
        int HourNumber,
        string? SubjectName,
        string? SubjectTypeName,
        string[] CurriculumTeachers,
        int? TeacherAbsenceId,
        string? ReplTeacher,
        string? ExtReplTeacher,
        bool? ReplTeacherIsNonSpecialist,
        bool? IsEmptyHour,
        string[] ExcusedStudentClassNumbers,
        string[] UnexcusedStudentClassNumbers,
        string[] LateStudentClassNumbers,
        string[] DplrAbsences,
        string[] DplrAttendances,
        string[] Topics,
        string? Location);
}

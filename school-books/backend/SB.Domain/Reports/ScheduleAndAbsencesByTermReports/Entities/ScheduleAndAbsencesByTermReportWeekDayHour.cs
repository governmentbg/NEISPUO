namespace SB.Domain;

using System;

public class ScheduleAndAbsencesByTermReportWeekDayHour
{
    // EF constructor
    private ScheduleAndAbsencesByTermReportWeekDayHour()
    {
    }

    internal ScheduleAndAbsencesByTermReportWeekDayHour(
        DateTime date,
        int hourNumber,
        bool? isEmptyHour,
        string? curriculumName,
        string? curriculumTeacherNames,
        string? excusedStudentClassNumbers,
        string? unexcusedStudentClassNumbers,
        string? lateStudentClassNumbers,
        string? dplrAbsenceStudentClassNumbers,
        string? dplrAttendanceStudentClassNumbers,
        string? topics)
    {
        this.Date = date;
        this.HourNumber = hourNumber;
        this.IsEmptyHour = isEmptyHour;
        this.CurriculumName = curriculumName;
        this.CurriculumTeacherNames = curriculumTeacherNames;
        this.ExcusedStudentClassNumbers = excusedStudentClassNumbers;
        this.UnexcusedStudentClassNumbers = unexcusedStudentClassNumbers;
        this.LateStudentClassNumbers = lateStudentClassNumbers;
        this.DplrAbsenceStudentClassNumbers = dplrAbsenceStudentClassNumbers;
        this.DplrAttendanceStudentClassNumbers = dplrAttendanceStudentClassNumbers;
        this.Topics = topics;
    }

    public int SchoolYear { get; private set; }
    public int ScheduleAndAbsencesByTermReportId { get; private set; }
    public int ScheduleAndAbsencesByTermReportWeekId { get; private set; }
    public DateTime Date { get; private set; }
    public int HourNumber { get; private set; }
    public bool? IsEmptyHour { get; private set; }
    public string? CurriculumName { get; private set; }
    public string? CurriculumTeacherNames { get; private set; }
    public string? ExcusedStudentClassNumbers { get; private set; }
    public string? UnexcusedStudentClassNumbers { get; private set; }
    public string? LateStudentClassNumbers { get; private set; }
    public string? DplrAbsenceStudentClassNumbers { get; private set; }
    public string? DplrAttendanceStudentClassNumbers { get; private set; }
    public string? Topics { get; private set; }
}

namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IScheduleAndAbsencesByTermReportsQueryRepository;

public class ScheduleAndAbsencesByTermReportWeekDay
{
    // EF constructor
    private ScheduleAndAbsencesByTermReportWeekDay()
    {
        this.DayName = null!;
    }

    internal ScheduleAndAbsencesByTermReportWeekDay(DateTime date, string dayName, bool isOffDay, bool isEmptyDay, GetWeeksForAddVODayHour[] hours)
    {
        this.Date = date;
        this.DayName = dayName;
        this.IsOffDay = isOffDay;
        this.IsEmptyDay = isEmptyDay;

        this.hours.AddRange(hours.Select(s => new ScheduleAndAbsencesByTermReportWeekDayHour(
            date,
            s.HourNumber,
            s.IsEmptyHour,
            s.CurriculumName,
            s.CurriculumTeacherNames,
            s.ExcusedStudentClassNumbers,
            s.UnexcusedStudentClassNumbers,
            s.LateStudentClassNumbers,
            s.DplrAbsenceStudentClassNumbers,
            s.DplrAttendanceStudentClassNumbers,
            s.Topics)));
    }

    public int SchoolYear { get; private set; }
    public int ScheduleAndAbsencesByTermReportId { get; private set; }
    public int ScheduleAndAbsencesByTermReportWeekId { get; private set; }
    public DateTime Date { get; private set; }
    public string DayName { get; private set; }
    public bool IsOffDay { get; private set; }
    public bool IsEmptyDay { get; private set; }

    // relations
    private readonly List<ScheduleAndAbsencesByTermReportWeekDayHour> hours = new();
    public IReadOnlyCollection<ScheduleAndAbsencesByTermReportWeekDayHour> Hours => this.hours.AsReadOnly();
}

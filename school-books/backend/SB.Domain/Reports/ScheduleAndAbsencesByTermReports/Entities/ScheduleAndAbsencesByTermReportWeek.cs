namespace SB.Domain;

using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IScheduleAndAbsencesByTermReportsQueryRepository;

public class ScheduleAndAbsencesByTermReportWeek
{
    // EF constructor
    private ScheduleAndAbsencesByTermReportWeek()
    {
        this.WeekName = null!;
    }

    public ScheduleAndAbsencesByTermReportWeek(
        string weekName,
        string? studentName,
        string? additionalActivities,
        GetWeeksForAddVODay[] days)
    {
        this.WeekName = weekName;
        this.StudentName = studentName;
        this.AdditionalActivities = additionalActivities;
        this.days.AddRange(days.Select(s => new ScheduleAndAbsencesByTermReportWeekDay(s.Date, s.DayName, s.IsOffDay, s.IsEmptyDay, s.Hours)));
    }

    public int SchoolYear { get; private set; }

    public int ScheduleAndAbsencesByTermReportId { get; private set; }

    public int ScheduleAndAbsencesByTermReportWeekId { get; private set; }

    public string WeekName { get; private set; }

    public string? StudentName { get; private set; }

    public string? AdditionalActivities { get; private set; }

    

    // relations
    private readonly List<ScheduleAndAbsencesByTermReportWeekDay> days = new();
    public IReadOnlyCollection<ScheduleAndAbsencesByTermReportWeekDay> Days => this.days.AsReadOnly();
}

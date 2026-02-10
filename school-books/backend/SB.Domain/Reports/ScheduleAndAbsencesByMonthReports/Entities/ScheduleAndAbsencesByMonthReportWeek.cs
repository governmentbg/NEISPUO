namespace SB.Domain;

using System.Collections.Generic;
using System.Linq;
using static SB.Domain.IScheduleAndAbsencesByMonthReportsQueryRepository;

public class ScheduleAndAbsencesByMonthReportWeek
{
    // EF constructor
    private ScheduleAndAbsencesByMonthReportWeek()
    {
        this.WeekName = null!;
    }

    public ScheduleAndAbsencesByMonthReportWeek(
        string weekName,
        string? studentName,
        string? additionalActivities,
        GetWeeksForAddVODay[] days)
    {
        this.WeekName = weekName;
        this.StudentName = studentName;
        this.AdditionalActivities = additionalActivities;
        this.days.AddRange(days.Select(s => new ScheduleAndAbsencesByMonthReportWeekDay(s.Date, s.DayName, s.IsOffDay, s.IsEmptyDay, s.Hours)));
    }

    public int SchoolYear { get; private set; }

    public int ScheduleAndAbsencesByMonthReportId { get; private set; }

    public int ScheduleAndAbsencesByMonthReportWeekId { get; private set; }

    public string WeekName { get; private set; }

    public string? StudentName { get; private set; }

    public string? AdditionalActivities { get; private set; }

    

    // relations
    private readonly List<ScheduleAndAbsencesByMonthReportWeekDay> days = new();
    public IReadOnlyCollection<ScheduleAndAbsencesByMonthReportWeekDay> Days => this.days.AsReadOnly();
}

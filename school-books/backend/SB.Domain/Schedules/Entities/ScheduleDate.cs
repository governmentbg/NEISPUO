namespace SB.Domain;

using System;
using System.Linq;
using SB.Common;

public class ScheduleDate
{
    // EF constructor
    protected ScheduleDate()
    {
        this.Schedule = null!;
    }

    internal ScheduleDate(
        Schedule schedule,
        DateTime date)
    {
        this.Schedule = schedule;
        this.Date = date;

        this.Year = DateExtensions.GetIsoWeekYear(date);
        this.WeekNumber = DateExtensions.GetIsoWeek(date);
        this.Day = DateExtensions.GetIsoDayOfWeek(date);

        if (!Schedule.ScheduleIsoDays.Contains(this.Day))
        {
            throw new DomainValidationException("Day is not in Schedule.ScheduleDays");
        }
    }

    public int SchoolYear { get; private set; }

    public int ScheduleId { get; private set; }

    public DateTime Date { get; private set; }

    public int Year { get; private set; }

    public int WeekNumber { get; private set; }

    public int Day { get; private set; }

    // relations
    public Schedule Schedule { get; private set; }
}

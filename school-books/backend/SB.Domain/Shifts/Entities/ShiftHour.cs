namespace SB.Domain;

using System;
using System.Linq;

public class ShiftHour
{
    // EF constructor
    private ShiftHour()
    {
    }

    internal ShiftHour(Shift shift, int day, int hourNumber, TimeSpan startTime, TimeSpan endTime)
    {
        if (!Schedule.ScheduleIsoDays.Contains(day))
        {
            throw new DomainValidationException("Day is not in Schedule.ScheduleDays");
        }

        this.Shift = shift;
        this.Day = day;
        this.HourNumber = hourNumber;
        this.StartTime = startTime;
        this.EndTime = endTime;
    }

    public int SchoolYear { get; private set; }

    public int ShiftId { get; private set; }

    public int Day { get; private set; }

    public int HourNumber { get; private set; }

    public TimeSpan StartTime { get; private set; }

    public TimeSpan EndTime { get; private set; }

    // relations
    public Shift Shift { get; private set; } = null!;
}

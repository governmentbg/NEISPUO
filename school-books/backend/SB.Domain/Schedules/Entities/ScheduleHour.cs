namespace SB.Domain;

using System.Linq;

public class ScheduleHour
{
    // EF constructor
    protected ScheduleHour()
    {
        this.Schedule = null!;
    }

    internal ScheduleHour(
        Schedule schedule,
        int day,
        int hourNumber,
        int curriculumId,
        string? location)
    {
        if (!Schedule.ScheduleIsoDays.Contains(day))
        {
            throw new DomainValidationException("Day is not in Schedule.ScheduleDays");
        }

        this.Schedule = schedule;
        this.Day = day;
        this.HourNumber = hourNumber;
        this.CurriculumId = curriculumId;
        this.Location = location;
    }

    public int SchoolYear { get; private set; }

    public int ScheduleId { get; private set; }

    public int Day { get; private set; }

    public int HourNumber { get; private set; }

    public int CurriculumId { get; private set; }

    public string? Location { get; private set; }

    // relations
    public Schedule Schedule { get; private set; }
}

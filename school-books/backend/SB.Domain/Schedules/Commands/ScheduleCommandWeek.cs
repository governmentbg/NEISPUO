namespace SB.Domain;

public record ScheduleCommandWeek
{
    public int Year { get; init; }

    public int WeekNumber { get; init; }

    public void Deconstruct(out int year, out int weekNumber)
        => (year, weekNumber) = (this.Year, this.WeekNumber);
}

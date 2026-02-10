namespace SB.Domain;

public record CreateShiftCommandHour
{
    public int? HourNumber { get; init; }
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }
}

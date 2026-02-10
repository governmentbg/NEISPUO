namespace SB.Domain;

public record CreateShiftCommandDay
{
    public int? Day { get; init; }
    public CreateShiftCommandHour[]? Hours { get; init; }
}

namespace SB.Domain;

public record CreateClassBookCommandClassBook
{
    public int? ClassId { get; init; }
    public string? ClassBookName { get; init; }
}

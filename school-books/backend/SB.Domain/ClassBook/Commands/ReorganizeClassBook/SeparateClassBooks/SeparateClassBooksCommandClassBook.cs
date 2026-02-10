namespace SB.Domain;

public record SeparateClassBooksCommandClassBook
{
    public int? ClassId { get; init; }
    public string? ClassBookName { get; init; }
}

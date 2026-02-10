namespace SB.Domain;

public record UpdateClassBookStudentNumbersCommandStudent
{
    public int? PersonId { get; init; }
    public int? ClassNumber { get; init; }
}

namespace SB.Domain;
public record CreateExamResultProtocolStudentCommandStudent
{
    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
}

namespace SB.Domain;
public record CreateQualificationExamResultProtocolStudentCommandStudent
{
    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
}

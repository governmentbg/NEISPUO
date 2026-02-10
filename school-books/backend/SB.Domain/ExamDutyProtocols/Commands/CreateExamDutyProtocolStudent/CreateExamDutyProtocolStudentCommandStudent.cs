namespace SB.Domain;
public record CreateExamDutyProtocolStudentCommandStudent
{
    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
}

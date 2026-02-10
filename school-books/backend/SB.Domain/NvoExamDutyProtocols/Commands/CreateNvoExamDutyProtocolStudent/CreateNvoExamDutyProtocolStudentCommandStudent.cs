namespace SB.Domain;
public record CreateNvoExamDutyProtocolStudentCommandStudent
{
    public int? ClassId { get; init; }
    public int? PersonId { get; init; }
}

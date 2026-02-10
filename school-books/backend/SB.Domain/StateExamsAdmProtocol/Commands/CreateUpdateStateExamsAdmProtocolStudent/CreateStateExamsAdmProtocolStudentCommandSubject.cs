namespace SB.Domain;
public record CreateStateExamsAdmProtocolStudentCommandSubject
{
    public int? SubjectId { get; init; }
    public int? SubjectTypeId { get; init; }
}

namespace SB.Domain;
public record CreateGradeChangeExamsAdmProtocolStudentCommandSubject
{
    public int? SubjectId { get; init; }
    public int? SubjectTypeId { get; init; }
}

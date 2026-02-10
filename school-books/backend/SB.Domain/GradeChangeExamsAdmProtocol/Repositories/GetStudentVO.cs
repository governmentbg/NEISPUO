namespace SB.Domain;
public partial interface IGradeChangeExamsAdmProtocolQueryRepository
{
    public record GetStudentVO(
        int ClassId,
        int PersonId,
        GetStudentVOSubject[] Subjects);

    public record GetStudentVOSubject(
        int SubjectId,
        int SubjectTypeId);
}

namespace SB.Domain;
public partial interface IGradeChangeExamsAdmProtocolQueryRepository
{
    public record GetStudentAllVO(
        string ClassName,
        int ClassId,
        int PersonId,
        string StudentName,
        string[] Subjects);
}

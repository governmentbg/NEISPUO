namespace SB.Domain;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetAbsencesVO(
        int CurriculumId,
        string CurriculumName,
        int LateAbsencesCount,
        int UnexcusedAbsencesCount,
        int ExcusedAbsencesCount);
}

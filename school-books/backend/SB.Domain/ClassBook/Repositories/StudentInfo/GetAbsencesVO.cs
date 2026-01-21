namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetAbsencesVO(
        int CurriculumId,
        string CurriculumName,
        bool CurriculumIsValid,
        int LateAbsencesCount,
        int UnexcusedAbsencesCount,
        int ExcusedAbsencesCount);
}

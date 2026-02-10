namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetAbsencesDplrVO(
        int CurriculumId,
        string CurriculumName,
        bool CurriculumIsValid,
        int Count);
}

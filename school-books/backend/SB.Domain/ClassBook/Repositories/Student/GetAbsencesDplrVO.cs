namespace SB.Domain;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetAbsencesDplrVO(
        int CurriculumId,
        string CurriculumName,
        int Count);
}

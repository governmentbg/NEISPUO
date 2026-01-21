namespace SB.Domain;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetRemarksVO(
        int CurriculumId,
        string CurriculumName,
        bool CurriculumIsValid,
        int BadRemarksCount,
        int GoodRemarksCount);
}

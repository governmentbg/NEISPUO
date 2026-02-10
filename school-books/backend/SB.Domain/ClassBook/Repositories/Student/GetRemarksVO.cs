namespace SB.Domain;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetRemarksVO(
        int CurriculumId,
        string CurriculumName,
        int BadRemarksCount,
        int GoodRemarksCount);
}

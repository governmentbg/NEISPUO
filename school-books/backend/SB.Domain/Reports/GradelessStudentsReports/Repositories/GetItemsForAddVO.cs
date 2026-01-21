namespace SB.Domain;

public partial interface IGradelessStudentsReportsQueryRepository
{
    public record GetItemsForAddVO(
        string ClassBookName,
        string StudentName,
        string CurriculumName);
}

namespace SB.Domain;

public partial interface IGradelessStudentsReportsQueryRepository
{
    public record GetItemsVO(
        string ClassBookName,
        string StudentName,
        string CurriculumName);
}

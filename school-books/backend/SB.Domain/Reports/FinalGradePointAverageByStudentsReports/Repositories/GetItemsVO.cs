namespace SB.Domain;

public partial interface IFinalGradePointAverageByStudentsReportsQueryRepository
{
    public record GetItemsVO(
        string ClassBookName,
        string StudentNames,
        bool IsTransferred,
        decimal FinalGradePointAverage);
}

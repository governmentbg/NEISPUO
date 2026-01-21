namespace SB.Domain;

public partial interface IFinalGradePointAverageByStudentsReportsQueryRepository
{
    public record GetItemsForAddVO(
        string ClassBookName,
        string StudentName,
        bool IsTransferred,
        decimal FinalGradePointAverage);
}

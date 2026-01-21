namespace SB.Domain;

public partial interface IRegularGradePointAverageByStudentsReportsQueryRepository
{
    public record GetItemsForAddVO(
        string ClassBookName,
        string StudentName,
        bool IsTransferred,
        string CurriculumInfo,
        decimal GradePointAverage,
        int TotalGradesCount,
        int PoorGradesCount,
        int FairGradesCount,
        int GoodGradesCount,
        int VeryGoodGradesCount,
        int ExcellentGradesCount,
        bool IsTotal);
}

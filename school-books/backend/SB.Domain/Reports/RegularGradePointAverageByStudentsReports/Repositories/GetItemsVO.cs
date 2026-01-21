namespace SB.Domain;

public partial interface IRegularGradePointAverageByStudentsReportsQueryRepository
{
    public record GetItemsVO(
        string ClassBookName,
        string StudentNames,
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

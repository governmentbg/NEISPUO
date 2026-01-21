namespace SB.Domain;

public partial interface IFinalGradePointAverageByClassesReportsQueryRepository
{
    public record GetItemsVO(
        string ClassBookName,
        string CurriculumInfo,
        int StudentsCount,
        int StudentsWithGradesCount,
        decimal StudentsWithGradesPercentage,
        decimal GradePointAverage,
        int TotalGradesCount,
        int PoorGradesCount,
        int FairGradesCount,
        int GoodGradesCount,
        int VeryGoodGradesCount,
        int ExcellentGradesCount,
        bool IsTotal);
}

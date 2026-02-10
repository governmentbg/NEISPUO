namespace SB.Domain;
using System;

public partial interface IRegularGradePointAverageByClassesReportsQueryRepository
{
    public record GetExcelDataVO(
        string Period,
        string? ClassBookNames,
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
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

namespace SB.Domain;
using System;

public partial interface IRegularGradePointAverageByStudentsReportsQueryRepository
{
    public record GetExcelDataVO(
        string Period,
        string? ClassBookNames,
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
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

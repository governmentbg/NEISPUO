namespace SB.Domain;
using System;

public partial interface IFinalGradePointAverageByStudentsReportsQueryRepository
{
    public record GetExcelDataVO(
        string Period,
        string? ClassBookNames,
        decimal? MinimalGradePointAverage,
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
        string ClassBookName,
        string StudentNames,
        bool IsTransferred,
        decimal FinalGradePointAverage);
}

namespace SB.Domain;

using System;

public partial interface IGradelessStudentsReportsQueryRepository
{
    public record GetExcelDataVO(
        bool OnlyFinalGrades,
        ReportPeriod Period,
        string? ClassBookNames,
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
        string ClassBookName,
        string StudentName,
        string CurriculumName);
}

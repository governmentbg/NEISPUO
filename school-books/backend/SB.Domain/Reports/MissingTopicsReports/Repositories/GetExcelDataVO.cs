namespace SB.Domain;

using System;

public partial interface IMissingTopicsReportsQueryRepository
{
    public record GetExcelDataVO(
        string Period,
        string? YearAndMonth,
        string? TeacherName,
        DateTime CreateDate,
        ExcelReportItemRow[] ReportItems);

    public record ExcelReportItemRow(
        DateTime Date,
        string ClassBookName,
        string CurriculumName,
        string[] TeachersNames);
}

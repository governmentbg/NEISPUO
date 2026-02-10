namespace SB.Domain;

using System;

public partial interface ISessionStudentsReportsQueryRepository
{
    public record GetExcelDataVO(
        DateTime CreateDate,
        GetExcelDataVOItem[] ReportItems);

    public record GetExcelDataVOItem(
        string StudentNames,
        bool IsTransferred,
        string ClassBookName,
        string? Session1CurriculumNames,
        string? Session2CurriculumNames,
        string? Session3CurriculumNames);
}

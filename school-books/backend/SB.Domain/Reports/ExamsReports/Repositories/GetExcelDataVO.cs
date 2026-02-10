namespace SB.Domain;

using System;

public partial interface IExamsReportsQueryRepository
{
    public record GetExcelDataVO(
        DateTime CreateDate,
        GetExcelDataVOItem[] Items);

    public record GetExcelDataVOItem(
        DateTime Date,
        string ClassBookName,
        BookExamType BookExamType,
        string CurriculumName,
        string CreatedBySysUserName);
}

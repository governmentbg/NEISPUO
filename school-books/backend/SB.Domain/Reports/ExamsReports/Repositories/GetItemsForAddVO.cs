namespace SB.Domain;

using System;

public partial interface IExamsReportsQueryRepository
{
    public record GetItemsForAddVO(
        DateTime Date,
        string ClassBookName,
        BookExamType BookExamType,
        string CurriculumName,
        string CreatedBySysUserName);
}

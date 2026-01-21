namespace SB.Domain;

using System;

public partial interface IExamsReportsQueryRepository
{
    public record GetItemsVO(
        DateTime Date,
        string ClassBookName,
        string BookExamType,
        string CurriculumName,
        string CreatedBySysUserName);
}

namespace SB.Domain;

using System;

public partial interface IMissingTopicsReportsQueryRepository
{
    public record GetItemsVO(
        DateTime Date,
        string ClassBookName,
        string CurriculumName,
        string TeacherName);
}

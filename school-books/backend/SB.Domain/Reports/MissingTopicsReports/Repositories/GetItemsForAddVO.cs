namespace SB.Domain;

using System;

public partial interface IMissingTopicsReportsQueryRepository
{
    public record GetItemsForAddVO(
        DateTime Date,
        string ClassBookName,
        string CurriculumName,
        string[] TeacherNames);
}

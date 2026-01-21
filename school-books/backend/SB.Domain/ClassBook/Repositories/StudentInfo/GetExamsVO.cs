namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetExamsVO(
        BookExamType BookExamType,
        string Subject,
        DateTime Date,
        string? Description);
}

namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetExamsVO(
        BookExamType BookExamType,
        string Subject,
        DateTime Date,
        string? Description);
}

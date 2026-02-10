namespace SB.Domain;

using System;

public partial interface IExamsQueryRepository
{
    public record GetAllVO(
        int ExamId,
        BookExamType BookExamType,
        string Subject,
        DateTime Date);
}

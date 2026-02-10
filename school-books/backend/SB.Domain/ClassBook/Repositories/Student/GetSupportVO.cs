namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetSupportVO(
        int SupportId,
        string Difficulties,
        string? Description,
        string? ExpectedResult,
        DateTime EndDate,
        string TeacherNames);
}

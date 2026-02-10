namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetSupportVO(
        int SupportId,
        string Difficulties,
        string? Description,
        string? ExpectedResult,
        DateTime EndDate,
        string TeacherNames);
}

namespace SB.Domain;

using System;

public partial interface IStudentClassBooksQueryRepository
{
    public record GetSanctionsVO(
        string SanctionType,
        DateTime StartDate,
        DateTime? EndDate,
        string? Description);
}

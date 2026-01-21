namespace SB.Domain;

using System;

public partial interface IStudentInfoClassBooksQueryRepository
{
    public record GetSanctionsVO(
        string SanctionType,
        DateTime StartDate,
        DateTime? EndDate,
        string? Description);
}

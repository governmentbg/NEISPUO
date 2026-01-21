namespace SB.Domain;

using System;

public partial interface ISanctionsQueryRepository
{
    public record GetAllVO(
        int SanctionId,
        int PersonId,
        string FullName,
        bool IsTransferred,
        string SanctionType,
        DateTime StartDate,
        DateTime? EndDate);
}

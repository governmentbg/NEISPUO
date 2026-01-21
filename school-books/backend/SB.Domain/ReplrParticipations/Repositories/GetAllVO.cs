namespace SB.Domain;

using System;

public partial interface IReplrParticipationsQueryRepository
{
    public record GetAllVO(
        int SanctionId,
        DateTime Date,
        string? Topic);
}

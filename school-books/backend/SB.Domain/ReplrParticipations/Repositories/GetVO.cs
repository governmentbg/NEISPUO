namespace SB.Domain;

using System;

public partial interface IReplrParticipationsQueryRepository
{
    public record GetVO(
        int ReplrParticipationId,
        int ReplrParticipationTypeId,
        DateTime Date,
        string? Topic,
        int? InstId,
        string Attendees);
}

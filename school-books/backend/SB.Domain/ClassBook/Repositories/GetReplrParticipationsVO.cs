namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetReplrParticipationsVO(
        string ReplrParticipationType,
        DateTime Date,
        string? Topic,
        string? InstName,
        string Attendees);
}

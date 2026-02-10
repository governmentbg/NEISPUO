namespace SB.Domain;

using System;

public partial interface ISupportsQueryRepository
{
    public record GetActivityVO(
        int SupportActivityTypeId,
        string? Target,
        string? Result,
        DateTime? Date);
}

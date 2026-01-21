namespace SB.Domain;

using System;

public partial interface ISupportsQueryRepository
{
    public record GetActivityAllVO(
        int SupportActivityId,
        string ActivityTypeDesc,
        DateTime? Date,
        string? Target,
        string? Result);
}

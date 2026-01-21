namespace SB.Domain;

using System;

public partial interface ISanctionsQueryRepository
{
    public record GetVO(
        int SanctionId,
        int PersonId,
        int SanctionTypeId,
        string OrderNumber,
        DateTime OrderDate,
        DateTime StartDate,
        DateTime? EndDate,
        string? Description,
        string? CancelOrderNumber,
        DateTime? CancelOrderDate,
        string? CancelReason,
        string? RuoOrderNumber,
        DateTime? RuoOrderDate);
}

namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetSanctionsVO(
        int? ClassNumber,
        string FullName,
        bool IsTransferred,
        string SanctionType,
        string OrderNumber,
        DateTime OrderDate,
        string? CancelOrderNumber,
        DateTime? CancelOrderDate);
}

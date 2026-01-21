namespace SB.Domain;

using System;

public partial interface IClassBookStudentPrintRepository
{
    public record GetStudentSanctionsVO(
        string SanctionType,
        string OrderNumber,
        DateTime OrderDate,
        string? CancelOrderNumber,
        DateTime? CancelOrderDate
    );
}

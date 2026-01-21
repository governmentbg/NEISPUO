namespace SB.Domain;

using System;

public record StudentSanctionsModel(
    StudentSanctionsModelSanction[] Sanctions
);

public record StudentSanctionsModelSanction(
    string SanctionType,
    string OrderNumber,
    DateTime OrderDate,
    string? CancelOrderNumber,
    DateTime? CancelOrderDate
);

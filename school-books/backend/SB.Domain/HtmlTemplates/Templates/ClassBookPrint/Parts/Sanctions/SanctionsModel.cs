namespace SB.Domain;

using System;

public record SanctionsModel(
    SanctionsModelStudent[] Students
);

public record SanctionsModelStudent(
    int? ClassNumber,
    string FullName,
    bool IsTransferred,
    SanctionsModelSanction[] Sanctions
);

public record SanctionsModelSanction(
    string SanctionType,
    string OrderNumber,
    DateTime OrderDate,
    string? CancelOrderNumber,
    DateTime? CancelOrderDate
);

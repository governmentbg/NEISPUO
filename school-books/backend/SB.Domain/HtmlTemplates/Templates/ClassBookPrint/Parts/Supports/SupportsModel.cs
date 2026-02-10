namespace SB.Domain;

using System;

public record SupportsModel(
    SupportsModelSupport[] Supports
);

public record SupportsModelSupport(
    string[] Students,
    string Difficulties,
    string? Description,
    string? ExpectedResult,
    DateTime EndDate,
    string[] Teachers,
    SupportsModelActivity[] Activities
);

public record SupportsModelActivity(
    string ActivityType,
    DateTime? Date,
    string? Target,
    string? Result
);

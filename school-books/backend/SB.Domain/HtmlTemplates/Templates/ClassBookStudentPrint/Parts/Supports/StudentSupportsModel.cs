namespace SB.Domain;

using System;

public record StudentSupportsModel(
    StudentSupportsModelSupport[] Supports
);

public record StudentSupportsModelSupport(
    string Difficulties,
    string? Description,
    string? ExpectedResult,
    DateTime EndDate,
    string[] Teachers,
    StudentSupportsModelActivity[] Activities
);

public record StudentSupportsModelActivity(
    string ActivityType,
    DateTime? Date,
    string? Target,
    string? Result
);

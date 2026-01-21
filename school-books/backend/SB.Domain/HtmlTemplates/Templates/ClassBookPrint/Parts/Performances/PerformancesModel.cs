namespace SB.Domain;

using System;

public record PerformancesModel(
    PerformancesModelPerformance[] Performances
);

public record PerformancesModelPerformance(
    string PerformanceType,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    string Location,
    string? StudentAwards
);

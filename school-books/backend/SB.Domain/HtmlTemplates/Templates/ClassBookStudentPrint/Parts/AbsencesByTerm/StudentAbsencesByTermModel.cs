namespace SB.Domain;

using System;

public record StudentAbsencesByTermModel(
    SchoolTerm Term,
    DateTime StartDate,
    DateTime EndDate,
    int ExcusedCount,
    decimal UnexcusedCount
);

namespace SB.Domain;

using System;

public partial interface IClassBookPrintRepository
{
    public record GetPerformancesVO(
        string PerformanceType,
        string Name,
        string Description,
        DateTime StartDate,
        DateTime EndDate,
        string Location,
        string? StudentAwards);
}

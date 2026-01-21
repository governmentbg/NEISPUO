namespace SB.Domain;

using System;

public partial interface IPerformancesQueryRepository
{
    public record GetVO(
        int PerformanceId,
        int PerformanceTypeId,
        string Name,
        string Description,
        DateTime StartDate,
        DateTime EndDate,
        string Location,
        string? StudentAwards);
}

namespace SB.Domain;

using System;

public partial interface IPerformancesQueryRepository
{
    public record GetAllVO(
        int SanctionId,
        string Name,
        string PerformanceType,
        DateTime StartDate,
        DateTime EndDate);
}

namespace SB.Domain;

using System;

public partial interface IPerformancesQueryRepository
{
    public record GetExcelDataVO(
        string? ClassBookName,
        string PerformanceTypeName,
        string Name,
        string Description,
        DateTime StartDate,
        DateTime EndDate,
        string Location,
        string? StudentAwards);
}

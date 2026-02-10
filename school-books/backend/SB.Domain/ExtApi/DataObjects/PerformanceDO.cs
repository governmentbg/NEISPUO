namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{PerformanceDO.Summary}}</summary>
public class PerformanceDO
{
    /// <summary>{{PerformanceDO.PerformanceId}}</summary>
    public int? PerformanceId { get; init; }

    /// <summary>{{PerformanceDO.PerformanceTypeId}}</summary>
    public int PerformanceTypeId { get; init; }

    /// <summary>{{PerformanceDO.Name}}</summary>
    [MaxLength(100)]
    public required string Name { get; init; }

    /// <summary>{{PerformanceDO.StartDate}}</summary>
    public DateTime StartDate { get; init; }

    /// <summary>{{PerformanceDO.EndDate}}</summary>
    public DateTime EndDate { get; init; }

    /// <summary>{{PerformanceDO.Description}}</summary>
    [MaxLength(10000)]
    public required string Description { get; init; }

    /// <summary>{{PerformanceDO.Location}}</summary>
    [MaxLength(100)]
    public required string Location { get; init; }

    /// <summary>{{PerformanceDO.StudentAwards}}</summary>
    [MaxLength(10000)]
    public string? StudentAwards { get; init; }
}

namespace SB.Domain;

using System.ComponentModel.DataAnnotations;

/// <summary>{{AdditionalActivityDO.Summary}}</summary>
public class AdditionalActivityDO
{
    /// <summary>{{AdditionalActivityDO.AdditionalActivityId}}</summary>
    public int? AdditionalActivityId { get; init; }

    /// <summary>{{AdditionalActivityDO.Year}}</summary>
    public int Year { get; init; }

    /// <summary>{{AdditionalActivityDO.WeekNumber}}</summary>
    public int WeekNumber { get; init; }

    /// <summary>{{AdditionalActivityDO.Activity}}</summary>
    [MaxLength(10000)]
    public required string Activity { get; init; }
}

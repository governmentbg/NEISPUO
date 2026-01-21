namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{ShiftDO.Summary}}</summary>
public class ShiftDO
{
    /// <summary>{{ShiftDO.ShiftId}}</summary>
    public int? ShiftId { get; init; }

    /// <summary>{{ShiftDO.Name}}</summary>
    [MaxLength(100)]
    public required string Name { get; init; }

    // TODO deprecated, remove nullability and make it required
    /// <summary>{{ShiftDO.IsMultiday}}</summary>
    public bool? IsMultiday { get; init; }

    /// <summary>{{ShiftDO.Hours}}</summary>
    public ShiftHourDO[] Hours { get; init; } = Array.Empty<ShiftHourDO>();
}

/// <summary>{{ShiftHourDO.Summary}}</summary>
public class ShiftHourDO
{
    // TODO deprecated, remove nullability and make it required
    /// <summary>{{ShiftHourDO.Day}}</summary>
    public int? Day { get; init; }

    /// <summary>{{ShiftHourDO.HourNumber}}</summary>
    public int HourNumber { get; init; }

    /// <summary>{{ShiftHourDO.StartTime}}</summary>
    public required string StartTime { get; init; }

    /// <summary>{{ShiftHourDO.EndTime}}</summary>
    public required string EndTime { get; init; }
}

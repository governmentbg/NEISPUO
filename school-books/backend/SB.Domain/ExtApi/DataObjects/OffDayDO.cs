namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{OffDayDO.Summary}}</summary>
public class OffDayDO
{
    /// <summary>{{OffDayDO.OffDayId}}</summary>
    public int? OffDayId { get; init; }

    /// <summary>{{OffDayDO.From}}</summary>
    public DateTime From { get; init; }

    /// <summary>{{OffDayDO.To}}</summary>
    public DateTime To { get; init; }

    /// <summary>{{OffDayDO.Description}}</summary>
    [MaxLength(1000)]
    public required string Description { get; init; }

    /// <summary>{{OffDayDO.IsForAllClasses}}</summary>
    public bool IsForAllClasses { get; init; }

    /// <summary>{{OffDayDO.BasicClassIds}}</summary>
    public int[] BasicClassIds { get; init; } = Array.Empty<int>();

    /// <summary>{{OffDayDO.ClassBookIds}}</summary>
    public int[] ClassBookIds { get; init; } = Array.Empty<int>();

    /// <summary>{{OffDayDO.IsPgOffProgramDay}}</summary>
    public bool? IsPgOffProgramDay { get; init; }
}

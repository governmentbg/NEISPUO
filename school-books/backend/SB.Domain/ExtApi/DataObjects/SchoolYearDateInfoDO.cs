namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

// TODO: OBSOLETE: rename to SchoolYearSettingsDO

/// <summary>{{SchoolYearDateInfoDO.Summary}}</summary>
public class SchoolYearDateInfoDO
{
    /// <summary>{{SchoolYearDateInfoDO.SchoolYearDateInfoId}}</summary>
    public int? SchoolYearDateInfoId { get; init; }

    /// <summary>{{SchoolYearDateInfoDO.SchoolYearStartDate}}</summary>
    public DateTime SchoolYearStartDate { get; init; }

    /// <summary>{{SchoolYearDateInfoDO.FirstTermEndDate}}</summary>
    public DateTime FirstTermEndDate { get; init; }

    /// <summary>{{SchoolYearDateInfoDO.SecondTermStartDate}}</summary>
    public DateTime SecondTermStartDate { get; init; }

    /// <summary>{{SchoolYearDateInfoDO.SchoolYearEndDate}}</summary>
    public DateTime SchoolYearEndDate { get; init; }

    /// <summary>{{SchoolYearDateInfoDO.Description}}</summary>
    [MaxLength(100)]
    public required string Description { get; init; }

    /// <summary>{{SchoolYearDateInfoDO.IsForAllClasses}}</summary>
    public bool IsForAllClasses { get; init; }

    /// <summary>{{SchoolYearDateInfoDO.BasicClassIds}}</summary>
    public int[] BasicClassIds { get; init; } = Array.Empty<int>();

    /// <summary>{{SchoolYearDateInfoDO.ClassBookIds}}</summary>
    public int[] ClassBookIds { get; init; } = Array.Empty<int>();
}

namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{RemarkDO.Summary}}</summary>
public class RemarkDO
{
    /// <summary>{{RemarkDO.RemarkId}}</summary>
    public int? RemarkId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{RemarkDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{RemarkDO.CurriculumId}}</summary>
    public int CurriculumId { get; init; }

    /// <summary>{{RemarkDO.Type}}</summary>
    public RemarkType Type { get; init; }

    /// <summary>{{RemarkDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{RemarkDO.Description}}</summary>
    [MaxLength(1000)]
    public required string Description { get; init; }
}

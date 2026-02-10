namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{IndividualWorkDO.Summary}}</summary>
public class IndividualWorkDO
{
    /// <summary>{{IndividualWorkDO.IndividualWorkId}}</summary>
    public int? IndividualWorkId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{IndividualWorkDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{IndividualWorkDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{IndividualWorkDO.IndividualWorkActivity}}</summary>
    [MaxLength(10000)]
    public required string IndividualWorkActivity { get; init; }
}

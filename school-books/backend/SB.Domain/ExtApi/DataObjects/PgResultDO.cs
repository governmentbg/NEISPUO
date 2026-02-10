namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{PgResultDO.Summary}}</summary>
public class PgResultDO
{
    /// <summary>{{PgResultDO.PgResultId}}</summary>
    public int? PgResultId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{PgResultDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{PgResultDO.SubjectId}}</summary>
    public int? SubjectId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? CurriculumId { get; init; }

    /// <summary>{{PgResultDO.StartSchoolYearResult}}</summary>
    [MaxLength(10000)]
    public string? StartSchoolYearResult { get; init; }

    /// <summary>{{PgResultDO.EndSchoolYearResult}}</summary>
    [MaxLength(10000)]
    public string? EndSchoolYearResult { get; init; }
}

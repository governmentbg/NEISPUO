namespace SB.Domain;

using System;

/// <summary>{{FirstGradeResultDO.Summary}}</summary>
public class FirstGradeResultDO
{
    /// <summary>{{FirstGradeResultDO.FirstGradeResultId}}</summary>
    public int? FirstGradeResultId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{FirstGradeResultDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{FirstGradeResultDO.QualitativeGrade}}</summary>
    public QualitativeGrade? QualitativeGrade { get; init; }

    /// <summary>{{FirstGradeResultDO.SpecialGrade}}</summary>
    public SpecialNeedsGrade? SpecialGrade { get; init; }
}

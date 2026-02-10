namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

/// <summary>{{GradeDO.Summary}}</summary>
[Keyless]
public class GradeDO
{
    /// <summary>{{GradeDO.GradeId}}</summary>
    public int? GradeId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{GradeDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{GradeDO.CurriculumId}}</summary>
    public int? CurriculumId { get; init; }

    /// <summary>{{GradeDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{GradeDO.Category}}</summary>
    public GradeCategory Category { get; init; }

    /// <summary>{{GradeDO.Type}}</summary>
    public GradeType Type { get; init; }

    /// <summary>{{GradeDO.DecimalGrade}}</summary>
    [Column(TypeName = "DECIMAL(3,2)")]
    public decimal? DecimalGrade { get; init; }

    /// <summary>{{GradeDO.QualitativeGrade}}</summary>
    public QualitativeGrade? QualitativeGrade { get; init; }

    /// <summary>{{GradeDO.SpecialGrade}}</summary>
    public SpecialNeedsGrade? SpecialGrade { get; init; }

    /// <summary>{{GradeDO.Comment}}</summary>
    [MaxLength(1000)]
    public string? Comment { get; init; }

    /// <summary>{{GradeDO.ScheduleLessonId}}</summary>
    public int? ScheduleLessonId { get; init; }

    /// <summary>{{GradeDO.Term}}</summary>
    public SchoolTerm? Term { get; init; }
}

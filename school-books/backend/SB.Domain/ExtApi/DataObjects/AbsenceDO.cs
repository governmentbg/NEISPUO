namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{AbsenceDO.Summary}}</summary>
public class AbsenceDO
{
    /// <summary>{{AbsenceDO.AbsenceId}}</summary>
    public int? AbsenceId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{AbsenceDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int CurriculumId { get; init; }

    /// <summary>{{AbsenceDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{AbsenceDO.Type}}</summary>
    public AbsenceType Type { get; init; }

    /// <summary>{{AbsenceDO.ExcusedReason}}</summary>
    public int? ExcusedReason { get; init; }

    /// <summary>{{AbsenceDO.ExcusedReasonComment}}</summary>
    [MaxLength(1000)]
    public string? ExcusedReasonComment { get; init; }

    /// <summary>{{AbsenceDO.ScheduleLessonId}}</summary>
    public int ScheduleLessonId { get; init; }
}

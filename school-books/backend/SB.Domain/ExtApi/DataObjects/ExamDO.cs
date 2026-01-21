namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{ExamDO.Summary}}</summary>
public class ExamDO
{
    /// <summary>{{ExamDO.ExamId}}</summary>
    public int? ExamId { get; init; }

    /// <summary>{{ExamDO.Type}}</summary>
    public BookExamType Type { get; init; }

    /// <summary>{{ExamDO.CurriculumId}}</summary>
    public int CurriculumId { get; init; }

    /// <summary>{{ExamDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{ExamDO.Description}}</summary>
    [MaxLength(1000)]
    public string? Description { get; init; }
}

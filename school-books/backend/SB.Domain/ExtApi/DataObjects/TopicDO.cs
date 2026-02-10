namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{TopicDO.Summary}}</summary>
public class TopicDO
{
    /// <summary>{{TopicDO.TopicId}}</summary>
    public int? TopicId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? CurriculumId { get; init; }

    /// <summary>{{TopicDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    /// <remarks>Да се използва полето Titles, Title ще бъде премахнато в бъдеще.</remarks>
    [MaxLength(1000)]
    [Obsolete("For removal")]
    public string? Title { get; init; }

    /// <summary>{{TopicDO.Titles}}</summary>
    [MaxLength(1000)]
    public string[]? Titles { get; init; }

    /// <summary>{{TopicDO.ScheduleLessonId}}</summary>
    public int ScheduleLessonId { get; init; }
}

namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{ParentMeetingDO.Summary}}</summary>
public class ParentMeetingDO
{
    /// <summary>{{ParentMeetingDO.ParentMeetingId}}</summary>
    public int? ParentMeetingId { get; init; }

    /// <summary>{{ParentMeetingDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{ParentMeetingDO.StartTime}}</summary>
    public required string StartTime { get; init; }

    /// <summary>{{ParentMeetingDO.Location}}</summary>
    [MaxLength(100)]
    public string? Location { get; init; }

    /// <summary>{{ParentMeetingDO.Title}}</summary>
    [MaxLength(100)]
    public required string Title { get; init; }

    /// <summary>{{ParentMeetingDO.Description}}</summary>
    [MaxLength(1000)]
    public string? Description { get; init; }
}

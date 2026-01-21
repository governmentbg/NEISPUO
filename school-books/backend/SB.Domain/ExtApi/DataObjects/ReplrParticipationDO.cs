namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{ReplrParticipationDO.Summary}}</summary>
public class ReplrParticipationDO
{
    /// <summary>{{ReplrParticipationDO.ReplrParticipationId}}</summary>
    public int? ReplrParticipationId { get; init; }

    /// <summary>{{ReplrParticipationDO.ReplrParticipationTypeId}}</summary>
    public int ReplrParticipationTypeId { get; init; }

    /// <summary>{{ReplrParticipationDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{ReplrParticipationDO.Topic}}</summary>
    [MaxLength(10000)]
    public string? Topic { get; init; }

    /// <summary>{{ReplrParticipationDO.Attendees}}</summary>
    [MaxLength(10000)]
    public required string Attendees { get; init; }

    /// <summary>{{ReplrParticipationDO.InstId}}</summary>
    public int? InstId { get; init; }
}

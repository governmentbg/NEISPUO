namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{SanctionDO.Summary}}</summary>
public class SanctionDO
{
    /// <summary>{{SanctionDO.SanctionId}}</summary>
    public int? SanctionId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{SanctionDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{SanctionDO.Type}}</summary>
    public int Type { get; init; }

    /// <summary>{{SanctionDO.OrderNumber}}</summary>
    [MaxLength(100)]
    public required string OrderNumber { get; init; }

    /// <summary>{{SanctionDO.OrderDate}}</summary>
    public DateTime OrderDate { get; init; }

    /// <summary>{{SanctionDO.StartDate}}</summary>
    public DateTime StartDate { get; init; }

    /// <summary>{{SanctionDO.EndDate}}</summary>
    public DateTime? EndDate { get; init; }

    /// <summary>{{SanctionDO.Description}}</summary>
    [MaxLength(1000)]
    public string? Description { get; init; }

    /// <summary>{{SanctionDO.CancelOrderNumber}}</summary>
    [MaxLength(100)]
    public string? CancelOrderNumber { get; init; }

    /// <summary>{{SanctionDO.CancelOrderDate}}</summary>
    public DateTime? CancelOrderDate { get; init; }

    /// <summary>{{SanctionDO.CancelReason}}</summary>
    [MaxLength(1000)]
    public string? CancelReason { get; init; }

    /// <summary>{{SanctionDO.RuoOrderNumber}}</summary>
    [MaxLength(100)]
    public string? RuoOrderNumber { get; init; }

    /// <summary>{{SanctionDO.RuoOrderDate}}</summary>
    public DateTime? RuoOrderDate { get; init; }
}

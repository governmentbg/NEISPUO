namespace SB.Domain;

using System.ComponentModel.DataAnnotations;

/// <summary>{{ExcusedReasonDO.Summary}}</summary>
public class ExcusedReasonDO
{
    /// <summary>{{ExcusedReasonDO.ExcusedReason}}</summary>
    public int? ExcusedReason { get; init; }

    /// <summary>{{ExcusedReasonDO.ExcusedReasonComment}}</summary>
    [MaxLength(1000)]
    public string? ExcusedReasonComment { get; init; }
}

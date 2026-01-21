namespace SB.Domain;

using System;
using System.ComponentModel.DataAnnotations;

/// <summary>{{AttendanceDO.Summary}}</summary>
public class AttendanceDO
{
    /// <summary>{{AttendanceDO.AttendanceId}}</summary>
    public int? AttendanceId { get; init; }

    /// <summary>{{Common.Obsolete}}</summary>
    [Obsolete("For removal")]
    public int? ClassId => 0;

    /// <summary>{{AttendanceDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{AttendanceDO.Date}}</summary>
    public DateTime Date { get; init; }

    /// <summary>{{AttendanceDO.Type}}</summary>
    public AttendanceType Type { get; init; }

    /// <summary>{{AttendanceDO.ExcusedReason}}</summary>
    public int? ExcusedReason { get; init; }

    /// <summary>{{AttendanceDO.ExcusedReasonComment}}</summary>
    [MaxLength(1000)]
    public string? ExcusedReasonComment { get; init; }
}

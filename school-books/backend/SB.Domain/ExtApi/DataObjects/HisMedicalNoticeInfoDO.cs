namespace SB.Domain;

using System;

/// <summary>{{HisMedicalNoticeInfoDO.Summary}}</summary>
public record HisMedicalNoticeInfoDO
{
    /// <summary>{{HisMedicalNoticeInfoDO.FromDate}}</summary>
    public DateTime FromDate { get; init; }

    /// <summary>{{HisMedicalNoticeInfoDO.ToDate}}</summary>
    public DateTime ToDate { get; init; }

    /// <summary>{{HisMedicalNoticeInfoDO.AuthoredOn}}</summary>
    public DateTime AuthoredOn { get; init; }
}

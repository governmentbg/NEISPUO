namespace SB.Domain;

using System;

/// <summary>{{MedicalNoticeBatchDO.Summary}}</summary>
public class MedicalNoticeBatchDO
{
    /// <summary>{{MedicalNoticeBatchDO.MedicalNotices}}</summary>
    public required MedicalNoticeDO[] MedicalNotices { get; init; }

    /// <summary>{{MedicalNoticeBatchDO.LastHisSyncTime}}</summary>
    public DateTime? LastHisSyncTime { get; init; }

    /// <summary>{{MedicalNoticeBatchDO.HasMore}}</summary>
    public bool HasMore { get; init; }
}

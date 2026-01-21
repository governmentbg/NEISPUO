namespace SB.Domain;

/// <summary>{{MedicalNoticeDO.Summary}}</summary>
public class MedicalNoticeDO
{
    /// <summary>{{MedicalNoticeDO.MedicalNoticeId}}</summary>
    public int MedicalNoticeId { get; init; }

    /// <summary>{{MedicalNoticeDO.PersonId}}</summary>
    public int PersonId { get; init; }

    /// <summary>{{MedicalNoticeDO.HisMedicalNotice}}</summary>
    public required HisMedicalNoticeDO HisMedicalNotice { get; init; }
}

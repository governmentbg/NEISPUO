namespace SB.Domain;

/// <summary>{{HisMedicalNoticeDO.Summary}}</summary>
public record HisMedicalNoticeDO
{
    /// <summary>{{HisMedicalNoticeDO.NrnMedicalNotice}}</summary>
    public required string NrnMedicalNotice { get; init; }

    /// <summary>{{HisMedicalNoticeDO.NrnExamination}}</summary>
    public required string NrnExamination { get; init; }

    /// <summary>{{HisMedicalNoticeDO.Patient}}</summary>
    public required HisMedicalNoticePatientDO Patient { get; init; }

    /// <summary>{{HisMedicalNoticeDO.Practitioner}}</summary>
    public required HisMedicalNoticePractitionerDO Practitioner { get; init; }

    /// <summary>{{HisMedicalNoticeDO.MedicalNotice}}</summary>
    public required HisMedicalNoticeInfoDO MedicalNotice { get; init; }
}

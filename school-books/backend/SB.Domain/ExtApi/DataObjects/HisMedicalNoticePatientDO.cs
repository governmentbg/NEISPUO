namespace SB.Domain;

/// <summary>{{HisMedicalNoticePatientDO.Summary}}</summary>
public record HisMedicalNoticePatientDO
{
    /// <summary>{{HisMedicalNoticePatientDO.IdentifierType}}</summary>
    public int IdentifierType { get; init; }

    /// <summary>{{HisMedicalNoticePatientDO.Identifier}}</summary>
    public required string Identifier { get; init; }

    /// <summary>{{HisMedicalNoticePatientDO.GivenName}}</summary>
    public required string GivenName { get; init; }

    /// <summary>{{HisMedicalNoticePatientDO.FamilyName}}</summary>
    public required string FamilyName { get; init; }
}

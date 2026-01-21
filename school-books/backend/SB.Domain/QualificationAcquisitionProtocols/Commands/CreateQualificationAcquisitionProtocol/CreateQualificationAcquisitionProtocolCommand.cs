namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateQualificationAcquisitionProtocolCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public QualificationAcquisitionProtocolType? ProtocolType { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public string? Profession { get; init; }

    public string? Speciality { get; init; }

    public int? QualificationDegreeId { get; init; }

    public DateTime? Date { get; init; }

    public string? CommissionNominationOrderNumber { get; init; }

    public DateTime? CommissionNominationOrderDate { get; init; }

    public int? DirectorPersonId { get; init; }

    public int? CommissionChairman { get; init; }

    public int[]? CommissionMembers { get; init; }

    [JsonIgnore]public string ObjectName => nameof(QualificationAcquisitionProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}

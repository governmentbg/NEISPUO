namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateStateExamsAdmProtocolCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public string? ProtocolNum { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public DateTime? CommissionMeetingDate { get; init; }

    public string? CommissionNominationOrderNumber { get; init; }

    public DateTime? CommissionNominationOrderDate { get; init; }

    public string? ExamSession { get; init; }

    public int? DirectorPersonId { get; init; }

    public int? CommissionChairman { get; init; }

    public int[]? CommissionMembers { get; init; }

    [JsonIgnore]public string ObjectName => nameof(StateExamsAdmProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}

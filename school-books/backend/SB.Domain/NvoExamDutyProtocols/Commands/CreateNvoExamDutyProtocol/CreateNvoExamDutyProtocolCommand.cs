namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateNvoExamDutyProtocolCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public int? BasicClassId { get; init; }

    public int? SubjectId { get; init; }

    public int? SubjectTypeId { get; init; }

    public DateTime? Date { get; init; }

    public string? RoomNumber { get; init; }

    public int? DirectorPersonId { get; init; }

    public int[]? SupervisorPersonIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(NvoExamDutyProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}

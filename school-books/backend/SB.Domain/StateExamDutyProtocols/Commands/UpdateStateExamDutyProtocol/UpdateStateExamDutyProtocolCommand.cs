namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateStateExamDutyProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? StateExamDutyProtocolId { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public string? OrderNumber { get; init; }

    public DateTime? OrderDate { get; init; }

    public string? SessionType { get; init; }

    public int[]? SupervisorPersonIds { get; init; }

    public int? SubjectId { get; init; }

    public int? SubjectTypeId { get; init; }

    public int? ModulesCount { get; init; }

    public string? RoomNumber { get; init; }

    public int? EduFormId { get; init; }

    public DateTime? Date { get; init; }

    [JsonIgnore]public string ObjectName => nameof(StateExamDutyProtocol);
    [JsonIgnore]public int? ObjectId => this.StateExamDutyProtocolId;
}

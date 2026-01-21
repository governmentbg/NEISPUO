namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveStateExamDutyProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? StateExamDutyProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(StateExamDutyProtocol);
    [JsonIgnore]public int? ObjectId => this.StateExamDutyProtocolId;
}

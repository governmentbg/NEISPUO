namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveNvoExamDutyProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? NvoExamDutyProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(NvoExamDutyProtocol);
    [JsonIgnore]public int? ObjectId => this.NvoExamDutyProtocolId;
}

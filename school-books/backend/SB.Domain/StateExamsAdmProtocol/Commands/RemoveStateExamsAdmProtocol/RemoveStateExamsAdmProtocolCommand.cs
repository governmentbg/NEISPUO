namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveStateExamsAdmProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? StateExamsAdmProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(StateExamsAdmProtocol);
    [JsonIgnore]public int? ObjectId => this.StateExamsAdmProtocolId;
}

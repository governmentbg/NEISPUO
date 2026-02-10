namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveGraduationThesisDefenseProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? GraduationThesisDefenseProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(GraduationThesisDefenseProtocol);
    [JsonIgnore]public int? ObjectId => this.GraduationThesisDefenseProtocolId;
}

namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveQualificationAcquisitionProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? QualificationAcquisitionProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(QualificationAcquisitionProtocol);
    [JsonIgnore]public int? ObjectId => this.QualificationAcquisitionProtocolId;
}

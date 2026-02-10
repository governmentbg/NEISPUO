namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveQualificationExamResultProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? QualificationExamResultProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(QualificationExamResultProtocol);
    [JsonIgnore]public int? ObjectId => this.QualificationExamResultProtocolId;
}

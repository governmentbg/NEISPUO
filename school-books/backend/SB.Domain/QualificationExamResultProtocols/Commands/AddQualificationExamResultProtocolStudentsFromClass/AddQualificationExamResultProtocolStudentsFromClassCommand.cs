namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record AddQualificationExamResultProtocolStudentsFromClassCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? QualificationExamResultProtocolId { get; init; }

    public int? ClassId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(QualificationExamResultProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}

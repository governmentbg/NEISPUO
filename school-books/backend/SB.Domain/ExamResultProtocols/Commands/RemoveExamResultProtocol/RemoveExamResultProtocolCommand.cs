namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveExamResultProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ExamResultProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ExamResultProtocol);
    [JsonIgnore]public int? ObjectId => this.ExamResultProtocolId;
}

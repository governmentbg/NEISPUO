namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveExamDutyProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ExamDutyProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ExamDutyProtocol);
    [JsonIgnore]public int? ObjectId => this.ExamDutyProtocolId;
}

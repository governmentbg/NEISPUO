namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveSkillsCheckExamResultProtocolEvaluatorCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? SkillsCheckExamResultProtocolId { get; init; }

    [JsonIgnore] public int? SkillsCheckExamResultProtocolEvaluatorId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SkillsCheckExamResultProtocolEvaluator);
    [JsonIgnore]public int? ObjectId => this.SkillsCheckExamResultProtocolEvaluatorId;
}

namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record CreateSkillsCheckExamResultProtocolEvaluatorCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? SkillsCheckExamResultProtocolId { get; init; }

    public string? Name { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SkillsCheckExamResultProtocolEvaluator);
    [JsonIgnore]public virtual int? ObjectId => null;
}

namespace SB.Domain;

using System.Text.Json.Serialization;

public record UpdateSkillsCheckExamResultProtocolEvaluatorCommand : CreateSkillsCheckExamResultProtocolEvaluatorCommand
{
    [JsonIgnore] public int? SkillsCheckExamResultProtocolEvaluatorId { get; init; }

    [JsonIgnore]public override int? ObjectId => this.SkillsCheckExamResultProtocolEvaluatorId;
}

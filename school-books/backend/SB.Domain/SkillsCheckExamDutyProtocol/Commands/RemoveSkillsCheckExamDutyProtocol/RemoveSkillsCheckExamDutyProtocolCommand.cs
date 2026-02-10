namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveSkillsCheckExamDutyProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? SkillsCheckExamDutyProtocolId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SkillsCheckExamDutyProtocol);
    [JsonIgnore]public int? ObjectId => this.SkillsCheckExamDutyProtocolId;
}

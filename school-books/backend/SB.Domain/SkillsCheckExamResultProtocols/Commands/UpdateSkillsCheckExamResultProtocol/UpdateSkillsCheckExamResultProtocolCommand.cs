namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateSkillsCheckExamResultProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? SkillsCheckExamResultProtocolId { get; init; }

    public string? ProtocolNumber { get; init; }

    public int? SubjectId { get; init; }

    public DateTime? Date { get; init; }

    public int? StudentsCapacity { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SkillsCheckExamResultProtocol);
    [JsonIgnore]public int? ObjectId => this.SkillsCheckExamResultProtocolId;
}

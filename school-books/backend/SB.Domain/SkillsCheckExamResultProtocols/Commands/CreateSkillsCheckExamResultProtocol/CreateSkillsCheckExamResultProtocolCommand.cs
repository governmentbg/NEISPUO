namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateSkillsCheckExamResultProtocolCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public string? ProtocolNumber { get; init; }

    public int? SubjectId { get; init; }

    public DateTime? Date { get; init; }

    public int? StudentsCapacity { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SkillsCheckExamResultProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}

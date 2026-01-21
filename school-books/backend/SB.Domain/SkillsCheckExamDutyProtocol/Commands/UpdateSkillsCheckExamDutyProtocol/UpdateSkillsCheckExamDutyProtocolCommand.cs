namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateSkillsCheckExamDutyProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? SkillsCheckExamDutyProtocolId { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public int? SubjectId { get; init; }

    public int? SubjectTypeId { get; init; }

    public DateTime? Date { get; init; }

    public int? DirectorPersonId { get; init; }

    public int[]? SupervisorPersonIds { get; init; }

    public int? StudentsCapacity { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SkillsCheckExamDutyProtocol);
    [JsonIgnore]public int? ObjectId => this.SkillsCheckExamDutyProtocolId;
}

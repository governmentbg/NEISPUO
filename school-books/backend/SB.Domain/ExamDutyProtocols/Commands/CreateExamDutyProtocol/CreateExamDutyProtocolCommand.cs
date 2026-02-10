namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateExamDutyProtocolCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public string? OrderNumber { get; init; }

    public DateTime? OrderDate { get; init; }

    public string? SessionType { get; init; }

    public int[]? ClassIds { get; init; }

    public int[]? SupervisorPersonIds { get; init; }

    public int? SubjectId { get; init; }

    public int? SubjectTypeId { get; init; }

    public int? ProtocolExamTypeId { get; init; }

    public int? ProtocolExamSubTypeId { get; init; }

    public string? GroupNum { get; init; }

    public int? EduFormId { get; init; }

    public DateTime? Date { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ExamDutyProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}

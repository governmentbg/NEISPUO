namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateExamResultProtocolCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public int? SubjectId { get; init; }

    public int? SubjectTypeId { get; init; }

    public string? SessionType { get; init; }

    public int[]? ClassIds { get; init; }

    public string? GroupNum { get; init; }

    public int? ProtocolExamTypeId { get; init; }

    public int? ProtocolExamSubTypeId { get; init; }

    public int? EduFormId { get; init; }

    public DateTime? Date { get; init; }

    public string? CommissionNominationOrderNumber { get; init; }

    public DateTime? CommissionNominationOrderDate { get; init; }

    public int? CommissionChairman { get; init; }

    public int[]? CommissionMembers { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ExamResultProtocol);
    [JsonIgnore]public virtual int? ObjectId => null;
}

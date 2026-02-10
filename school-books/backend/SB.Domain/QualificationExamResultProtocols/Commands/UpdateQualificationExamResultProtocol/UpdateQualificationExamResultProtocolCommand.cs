namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateQualificationExamResultProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? QualificationExamResultProtocolId { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public string? SessionType { get; init; }

    public string? Profession { get; init; }

    public string? Speciality { get; init; }

    public int? QualificationDegreeId { get; init; }

    public int[]? ClassIds { get; init; }

    public string? GroupNum { get; init; }

    public int? QualificationExamTypeId { get; init; }

    public int? EduFormId { get; init; }

    public DateTime? Date { get; init; }

    public string? CommissionNominationOrderNumber { get; init; }

    public DateTime? CommissionNominationOrderDate { get; init; }

    public int? CommissionChairman { get; init; }

    public int[]? CommissionMembers { get; init; }

    [JsonIgnore]public string ObjectName => nameof(QualificationExamResultProtocol);
    [JsonIgnore]public int? ObjectId => this.QualificationExamResultProtocolId;
}

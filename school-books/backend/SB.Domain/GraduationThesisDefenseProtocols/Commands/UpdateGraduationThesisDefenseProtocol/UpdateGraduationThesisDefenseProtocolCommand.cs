namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateGraduationThesisDefenseProtocolCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? GraduationThesisDefenseProtocolId { get; init; }

    public string? ProtocolNumber { get; init; }

    public DateTime? ProtocolDate { get; init; }

    public string? SessionType { get; init; }

    public int? EduFormId { get; init; }

    public DateTime? CommissionMeetingDate { get; init; }

    public string? DirectorOrderNumber { get; init; }

    public DateTime? DirectorOrderDate { get; init; }

    public int? DirectorPersonId { get; init; }

    public int? CommissionChairman { get; init; }

    public int[]? CommissionMembers { get; init; }

    public int? Section1StudentsCapacity { get; init; }

    public int? Section2StudentsCapacity { get; init; }

    public int? Section3StudentsCapacity { get; init; }

    public int? Section4StudentsCapacity { get; init; }

    [JsonIgnore]public string ObjectName => nameof(GraduationThesisDefenseProtocol);
    [JsonIgnore]public int? ObjectId => this.GraduationThesisDefenseProtocolId;
}

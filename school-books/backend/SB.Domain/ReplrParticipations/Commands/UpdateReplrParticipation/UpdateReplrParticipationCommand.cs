namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record UpdateReplrParticipationCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ReplrParticipationId { get; init; }

    public int? ReplrParticipationTypeId { get; init; }
    public DateTime? Date { get; init; }
    public string? Topic { get; init; }
    public int? InstitutionId { get; init; }
    public string? Attendees { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ReplrParticipation);
    [JsonIgnore]public int? ObjectId => this.ReplrParticipationId;
}

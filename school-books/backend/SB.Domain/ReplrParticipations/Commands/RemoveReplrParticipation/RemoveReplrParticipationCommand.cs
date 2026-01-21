namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveReplrParticipationCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? ReplrParticipationId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ReplrParticipation);
    [JsonIgnore]public int? ObjectId => this.ReplrParticipationId;
}

namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveSanctionCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? SanctionId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Sanction);
    [JsonIgnore]public int? ObjectId => this.SanctionId;
}

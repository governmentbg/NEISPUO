namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemovePublicationCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? PublicationId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Publication);
    [JsonIgnore]public int? ObjectId => this.PublicationId;
}

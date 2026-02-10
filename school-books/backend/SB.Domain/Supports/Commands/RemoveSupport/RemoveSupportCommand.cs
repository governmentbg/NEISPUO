namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveSupportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? SupportId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Support);
    [JsonIgnore]public int? ObjectId => this.SupportId;
}

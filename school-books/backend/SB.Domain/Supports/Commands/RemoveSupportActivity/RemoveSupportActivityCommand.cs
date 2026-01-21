namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveSupportActivityCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? SupportId { get; init; }
    [JsonIgnore]public int? SupportActivityId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SupportActivity);
    [JsonIgnore]public int? ObjectId => this.SupportActivityId;
}

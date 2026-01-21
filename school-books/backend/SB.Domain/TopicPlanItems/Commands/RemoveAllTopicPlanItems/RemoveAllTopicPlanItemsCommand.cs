namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveAllTopicPlanItemsCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? TopicPlanId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(TopicPlanItem);
    [JsonIgnore]public int? ObjectId => null;
}

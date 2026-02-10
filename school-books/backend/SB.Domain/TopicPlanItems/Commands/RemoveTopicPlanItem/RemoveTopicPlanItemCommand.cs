namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveTopicPlanItemCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? TopicPlanItemId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(TopicPlanItem);
    [JsonIgnore]public int? ObjectId => this.TopicPlanItemId;
}

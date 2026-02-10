namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateTopicPlanItemCommand : IRequest<int>, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? TopicPlanItemId { get; init; }

    public int? Number { get; init; }
    public string? Title { get; init; }
    public string? Note { get; init; }

    [JsonIgnore] public string ObjectName => nameof(TopicPlanItem);

    [JsonIgnore]public int? ObjectId => this.TopicPlanItemId;
}

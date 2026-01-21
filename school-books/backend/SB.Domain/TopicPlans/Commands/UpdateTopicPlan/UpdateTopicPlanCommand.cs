namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdateTopicPlanCommand : IRequest<int>, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? TopicPlanId { get; init; }

    public string? Name { get; init; }
    public int? BasicClassId { get; init; }
    public int? SubjectId { get; init; }
    public int? SubjectTypeId { get; init; }
    public int? TopicPlanPublisherId { get; init; }
    public string? TopicPlanPublisherOther { get; init; }

    [JsonIgnore] public string ObjectName => nameof(TopicPlan);
    [JsonIgnore] public int? ObjectId => this.TopicPlanId;
}

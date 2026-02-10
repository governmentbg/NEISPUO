namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record CreateTopicPlanItemCommand : IRequest<int>, IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? TopicPlanId { get; init; }
    public int? Number { get; init; }
    public string? Title { get; init; }
    public string? Note { get; init; }

    [JsonIgnore]public string ObjectName => nameof(TopicPlanItem);
    [JsonIgnore]public virtual int? ObjectId => null;
}

namespace SB.Domain;
using System.Text.Json.Serialization;
using MediatR;

public record RemoveTopicDplrCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? ClassBookId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? TopicDplrId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(TopicDplr);
    [JsonIgnore] public virtual int? ObjectId => null;
}

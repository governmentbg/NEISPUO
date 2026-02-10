namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Text.Json.Serialization;

public record RemoveTopicsCommand : IRequest, IAuditedUpdateMultipleCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public RemoveTopicsCommandTopic[]? Topics { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Topic);
    [JsonIgnore]public virtual int? ObjectId => null;
    [JsonIgnore]public int[] ObjectIds =>
        this.Topics
            ?.Select(x => x.TopicId!.Value)
            ?.ToArray()
        ?? Array.Empty<int>();
}

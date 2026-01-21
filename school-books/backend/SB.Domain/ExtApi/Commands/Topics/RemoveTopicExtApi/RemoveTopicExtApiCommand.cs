namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveTopicExtApiCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? TopicId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Topic);
    [JsonIgnore]public virtual int? ObjectId => this.TopicId;
}

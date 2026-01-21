namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateTopicsCommand : IAuditedCreateMultipleCommand
{
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public CreateTopicsCommandTopic[]? Topics { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Topic);
    [JsonIgnore]public virtual int? ObjectId => null;
}

namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateTopicExtApiCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public string? Title { get; init; }
    public string[]? Titles { get; init; }
    public int[]? StudentPersonIds { get; init; }
    public DateTime? Date { get; init; }
    public int? ScheduleLessonId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Topic);
    [JsonIgnore]public virtual int? ObjectId => null;
}

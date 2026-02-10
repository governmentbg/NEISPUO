namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateTopicDplrCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public DateTime? Date { get; init; }
    public int? Day { get; init; }
    public int? HourNumber { get; init; }
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }
    public int? CurriculumId { get; init; }
    public string? Location { get; init; }
    public string? Title { get; init; }
    public int[]? StudentPersonIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(TopicDplr);
    [JsonIgnore]public virtual int? ObjectId => null;
}

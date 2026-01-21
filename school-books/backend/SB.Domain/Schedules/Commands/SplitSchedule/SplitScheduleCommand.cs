namespace SB.Domain;

using System.Text.Json.Serialization;

public record SplitScheduleCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? ScheduleId { get; init; }

    public ScheduleCommandWeek[]? Weeks { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Schedule);
    [JsonIgnore]public virtual int? ObjectId => null;
}

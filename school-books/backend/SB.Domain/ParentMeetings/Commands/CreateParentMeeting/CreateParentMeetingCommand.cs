namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateParentMeetingCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public DateTime? Date { get; init; }

    public string? StartTime { get; init; }

    public string? Location { get; init; }

    public string? Title { get; init; }

    public string? Description { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ParentMeeting);
    [JsonIgnore]public virtual int? ObjectId => null;
}

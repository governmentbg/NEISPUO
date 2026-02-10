namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateOffDayCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public DateTime? From { get; init; }

    public DateTime? To { get; init; }

    public string? Description { get; init; }

    public bool? IsForAllClasses { get; init; }

    public int[]? BasicClassIds { get; init; }

    public int[]? ClassBookIds { get; init; }

    public bool? IsPgOffProgramDay { get; init; }

    [JsonIgnore]public string ObjectName => nameof(OffDay);
    [JsonIgnore]public virtual int? ObjectId => null;
}

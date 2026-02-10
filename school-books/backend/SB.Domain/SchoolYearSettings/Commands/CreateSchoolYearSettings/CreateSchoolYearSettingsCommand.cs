namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateSchoolYearSettingsCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public DateTime? SchoolYearStartDate { get; init; }

    public DateTime? FirstTermEndDate { get; init; }

    public DateTime? SecondTermStartDate { get; init; }

    public DateTime? SchoolYearEndDate { get; init; }

    public string? Description { get; init; }

    public bool? HasFutureEntryLock { get; init; }

    public int? PastMonthLockDay { get; init; }

    public bool? IsForAllClasses { get; init; }

    public int[]? BasicClassIds { get; init; }

    public int[]? ClassBookIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SchoolYearSettings);
    [JsonIgnore]public virtual int? ObjectId => null;
}

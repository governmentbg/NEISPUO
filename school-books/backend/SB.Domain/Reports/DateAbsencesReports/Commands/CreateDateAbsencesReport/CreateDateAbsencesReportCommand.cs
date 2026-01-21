namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateDateAbsencesReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public DateTime? ReportDate { get; init; }
    public int[]? ClassBookIds { get; init; }
    public int[]? ShiftIds { get; init; }
    public bool? IsUnited { get; init; }

    [JsonIgnore]public string ObjectName => nameof(DateAbsencesReport);
    [JsonIgnore]public virtual int? ObjectId => null;
}

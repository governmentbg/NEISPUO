namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateAbsenceExtApiCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public DateTime? Date { get; init; }
    public AbsenceType? Type { get; init; }
    public int? ExcusedReason { get; init; }
    public string? ExcusedReasonComment { get; init; }
    public int? ScheduleLessonId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Absence);
    [JsonIgnore]public virtual int? ObjectId => null;
}

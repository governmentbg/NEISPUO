namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateScheduleAndAbsencesByTermReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public SchoolTerm? Term { get; init; }
    public int? ClassBookId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ScheduleAndAbsencesByTermReport);
    [JsonIgnore]public virtual int? ObjectId => null;
}

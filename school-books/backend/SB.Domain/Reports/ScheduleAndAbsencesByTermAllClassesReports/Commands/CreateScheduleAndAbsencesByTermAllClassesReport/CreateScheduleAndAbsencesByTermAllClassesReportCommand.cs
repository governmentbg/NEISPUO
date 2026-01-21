namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateScheduleAndAbsencesByTermAllClassesReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public SchoolTerm? Term { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ScheduleAndAbsencesByTermAllClassesReport);
    [JsonIgnore]public virtual int? ObjectId => null;
}

namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateScheduleAndAbsencesByMonthReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public int? Year { get; init; }
    public int? Month { get; init; }
    public int? ClassBookId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ScheduleAndAbsencesByMonthReport);
    [JsonIgnore]public virtual int? ObjectId => null;
}

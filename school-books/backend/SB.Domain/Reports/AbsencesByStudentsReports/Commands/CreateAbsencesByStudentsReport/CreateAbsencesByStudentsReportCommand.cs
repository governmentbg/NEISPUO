namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateAbsencesByStudentsReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public ReportPeriod? Period { get; set; }
    public int[]? ClassBookIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(AbsencesByStudentsReport);
    [JsonIgnore]public virtual int? ObjectId => null;
}

namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateFinalGradePointAverageByStudentsReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public ReportPeriod? Period { get; set; }
    public int[]? ClassBookIds { get; init; }
    public decimal? MinimumGradePointAverage { get; init; }

    [JsonIgnore] public string ObjectName => nameof(FinalGradePointAverageByStudentsReport);
    [JsonIgnore] public virtual int? ObjectId => null;
}

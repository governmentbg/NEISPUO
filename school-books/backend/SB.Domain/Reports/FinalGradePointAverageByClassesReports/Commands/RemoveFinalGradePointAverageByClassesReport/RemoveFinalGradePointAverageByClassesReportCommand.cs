namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record class RemoveFinalGradePointAverageByClassesReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? FinalGradePointAverageByClassesReportId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(FinalGradePointAverageByClassesReport);
    [JsonIgnore] public int? ObjectId => this.FinalGradePointAverageByClassesReportId;
}

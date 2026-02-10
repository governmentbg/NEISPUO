namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record class RemoveRegularGradePointAverageByClassesReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? RegularGradePointAverageByClassesReportId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(RegularGradePointAverageByClassesReport);
    [JsonIgnore] public int? ObjectId => this.RegularGradePointAverageByClassesReportId;
}

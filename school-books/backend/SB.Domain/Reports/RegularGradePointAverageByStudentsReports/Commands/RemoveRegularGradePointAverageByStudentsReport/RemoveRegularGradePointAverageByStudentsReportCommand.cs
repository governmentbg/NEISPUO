namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record class RemoveRegularGradePointAverageByStudentsReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? RegularGradePointAverageByStudentsReportId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(RegularGradePointAverageByStudentsReport);
    [JsonIgnore] public int? ObjectId => this.RegularGradePointAverageByStudentsReportId;
}

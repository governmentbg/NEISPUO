namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record class RemoveAbsencesByClassesReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? AbsencesByClassesReportId { get; init; }

    [JsonIgnore] public string ObjectName => nameof(AbsencesByClassesReport);
    [JsonIgnore] public int? ObjectId => this.AbsencesByClassesReportId;
}

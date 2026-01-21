namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveScheduleAndAbsencesByTermAllClassesReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ScheduleAndAbsencesByTermAllClassesReportId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ScheduleAndAbsencesByTermAllClassesReport);
    [JsonIgnore]public int? ObjectId => this.ScheduleAndAbsencesByTermAllClassesReportId;
}

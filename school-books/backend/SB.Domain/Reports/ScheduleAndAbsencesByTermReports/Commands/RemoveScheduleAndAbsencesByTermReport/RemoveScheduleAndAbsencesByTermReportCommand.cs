namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveScheduleAndAbsencesByTermReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ScheduleAndAbsencesByTermReportId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ScheduleAndAbsencesByTermReport);
    [JsonIgnore]public int? ObjectId => this.ScheduleAndAbsencesByTermReportId;
}

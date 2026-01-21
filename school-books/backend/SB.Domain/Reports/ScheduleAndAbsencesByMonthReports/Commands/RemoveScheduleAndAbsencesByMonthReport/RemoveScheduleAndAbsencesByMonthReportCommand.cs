namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveScheduleAndAbsencesByMonthReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? ScheduleAndAbsencesByMonthReportId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ScheduleAndAbsencesByMonthReport);
    [JsonIgnore]public int? ObjectId => this.ScheduleAndAbsencesByMonthReportId;
}

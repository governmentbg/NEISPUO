namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveLectureSchedulesReportCommand : IRequest, IAuditedCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? LectureSchedulesReportId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(LectureSchedulesReport);
    [JsonIgnore]public int? ObjectId => this.LectureSchedulesReportId;
}

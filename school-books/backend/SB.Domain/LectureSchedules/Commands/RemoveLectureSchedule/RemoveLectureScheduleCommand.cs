namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record RemoveLectureScheduleCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? LectureScheduleId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(LectureSchedule);
    [JsonIgnore]public int? ObjectId => this.LectureScheduleId;
}

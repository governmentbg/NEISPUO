namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record UpdateIsVerifiedScheduleLessonCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public UpdateIsVerifiedScheduleLessonCommandScheduleLesson[]? ScheduleLessons { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ScheduleLesson);
    [JsonIgnore]public int? ObjectId => null;
}

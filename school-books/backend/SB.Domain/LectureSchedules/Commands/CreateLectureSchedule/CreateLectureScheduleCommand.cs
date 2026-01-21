namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateLectureScheduleCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? LectureScheduleId { get; init; }

    public int? TeacherPersonId { get; init; }
    public string? OrderNumber { get; init; }
    public DateTime? OrderDate { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public int[]? ScheduleLessonIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(LectureSchedule);
    [JsonIgnore]public int? ObjectId => this.LectureScheduleId;
}

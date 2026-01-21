namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateLectureSchedulesReportCommand : IAuditedCreateCommand
{
    [JsonIgnore] public int? SchoolYear { get; init; }
    [JsonIgnore] public int? InstId { get; init; }
    [JsonIgnore] public int? SysUserId { get; init; }

    public LectureSchedulesReportPeriod? Period { get; set; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public int? TeacherPersonId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(LectureSchedulesReport);
    [JsonIgnore]public virtual int? ObjectId => null;
}

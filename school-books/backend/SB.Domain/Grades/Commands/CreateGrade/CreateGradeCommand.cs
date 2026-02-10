namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateGradeCommand : IAuditedCreateMultipleCommand
{
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? CurriculumId { get; init; }
    public GradeType? Type { get; init; }
    public DateTime? Date { get; init; }
    public SchoolTerm? Term { get; init; }
    public int? ScheduleLessonId { get; init; }
    public int? TeacherAbsenceId { get; init; }
    public CreateGradeCommandStudent[]? Students { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Grade);
    [JsonIgnore]public virtual int? ObjectId => null;
}

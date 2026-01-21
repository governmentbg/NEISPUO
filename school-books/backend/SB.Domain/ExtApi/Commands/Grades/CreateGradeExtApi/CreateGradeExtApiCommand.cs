namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateGradeExtApiCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public int? CurriculumId { get; init; }
    public DateTime? Date { get; init; }
    public GradeCategory? Category { get; init; }
    public GradeType? Type { get; init; }
    public decimal? DecimalGrade { get; init; }
    public QualitativeGrade? QualitativeGrade { get; init; }
    public SpecialNeedsGrade? SpecialGrade { get; init; }
    public string? Comment { get; init; }
    public int? ScheduleLessonId { get; init; }
    public SchoolTerm? Term { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Grade);
    [JsonIgnore]public virtual int? ObjectId => null;
}

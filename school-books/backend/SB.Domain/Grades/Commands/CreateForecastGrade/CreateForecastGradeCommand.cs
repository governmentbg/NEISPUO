namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateForecastGradeCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? CurriculumId { get; init; }
    public int? PersonId { get; init; }
    public GradeCategory? Category { get; init; }
    public GradeType? Type { get; init; }
    public SchoolTerm? Term { get; init; }
    public decimal? DecimalGrade { get; init; }
    public QualitativeGrade? QualitativeGrade { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Grade);
    [JsonIgnore]public virtual int? ObjectId => null;
}

namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreatePgResultCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public int? CurriculumId { get; init; }
    public int? SubjectId { get; init; }
    public string? StartSchoolYearResult { get; init; }
    public string? EndSchoolYearResult { get; init; }

    [JsonIgnore]public string ObjectName => nameof(PgResult);
    [JsonIgnore]public virtual int? ObjectId => null;
}

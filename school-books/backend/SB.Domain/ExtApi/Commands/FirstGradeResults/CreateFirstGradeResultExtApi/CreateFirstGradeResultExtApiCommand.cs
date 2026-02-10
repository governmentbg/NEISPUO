namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateFirstGradeResultExtApiCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public QualitativeGrade? QualitativeGrade { get; init; }
    public SpecialNeedsGrade? SpecialGrade { get; init; }

    [JsonIgnore]public string ObjectName => nameof(FirstGradeResult);
    [JsonIgnore]public virtual int? ObjectId => null;
}

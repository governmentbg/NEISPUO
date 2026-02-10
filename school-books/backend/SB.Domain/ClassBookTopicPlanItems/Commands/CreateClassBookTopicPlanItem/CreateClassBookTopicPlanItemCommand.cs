namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateClassBookTopicPlanItemCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore] public int? CurriculumId { get; init; }

    public int? Number { get; init; }
    public string? Title { get; init; }
    public string? Note { get; init; }
    public bool? Taken { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ClassBookTopicPlanItem);
    [JsonIgnore]public virtual int? ObjectId => null;
}

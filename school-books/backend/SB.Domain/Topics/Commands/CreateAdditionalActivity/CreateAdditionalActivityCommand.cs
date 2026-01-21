namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateAdditionalActivityCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? Year { get; init; }
    public int? WeekNumber { get; init; }
    public string? Activity { get; init; }

    [JsonIgnore]public string ObjectName => nameof(AdditionalActivity);
    [JsonIgnore]public virtual int? ObjectId => null;
}

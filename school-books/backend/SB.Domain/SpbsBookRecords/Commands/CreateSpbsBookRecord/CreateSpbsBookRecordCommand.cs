namespace SB.Domain;
using System.Text.Json.Serialization;

public record CreateSpbsBookRecordCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? ClassId { get; init; }
    public int? PersonId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SpbsBookRecord);
    [JsonIgnore]public virtual int? ObjectId => null;
}

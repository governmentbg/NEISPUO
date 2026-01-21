namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateShiftCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public bool? IsExternal { get; init; }

    public string? Name { get; init; }
    public bool? IsMultiday { get; init; }
    public CreateShiftCommandDay[]? Days { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Shift);
    [JsonIgnore]public virtual int? ObjectId => null;
}

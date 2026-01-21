namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreatePerformanceCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PerformanceTypeId { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Location { get; init; }
    public string? StudentAwards { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Performance);
    [JsonIgnore]public virtual int? ObjectId => null;
}

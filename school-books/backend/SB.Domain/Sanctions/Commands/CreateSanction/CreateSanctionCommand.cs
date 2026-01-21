namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateSanctionCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public int? SanctionTypeId { get; init; }
    public string? OrderNumber { get; init; }
    public DateTime? OrderDate { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Description { get; init; }
    public string? CancelOrderNumber { get; init; }
    public DateTime? CancelOrderDate { get; init; }
    public string? CancelReason { get; init; }
    public string? RuoOrderNumber { get; init; }
    public DateTime? RuoOrderDate { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Sanction);
    [JsonIgnore]public virtual int? ObjectId => null;
}

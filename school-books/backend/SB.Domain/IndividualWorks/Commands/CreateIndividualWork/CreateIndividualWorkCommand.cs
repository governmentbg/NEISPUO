namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateIndividualWorkCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public DateTime? Date { get; init; }
    public string? IndividualWorkActivity { get; init; }

    [JsonIgnore]public string ObjectName => nameof(IndividualWork);
    [JsonIgnore]public virtual int? ObjectId => null;
}

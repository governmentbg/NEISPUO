namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record UpdatePgResultCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? PgResultId { get; init; }

    public string? StartSchoolYearResult { get; init; }
    public string? EndSchoolYearResult { get; init; }

    [JsonIgnore]public string ObjectName => nameof(PgResult);
    [JsonIgnore]public int? ObjectId => this.PgResultId;
}

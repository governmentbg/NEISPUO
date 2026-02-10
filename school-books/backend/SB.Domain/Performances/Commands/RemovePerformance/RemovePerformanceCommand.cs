namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemovePerformanceCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? PerformanceId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Performance);
    [JsonIgnore]public int? ObjectId => this.PerformanceId;
}

namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveAdditionalActivityCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? AdditionalActivityId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(AdditionalActivity);
    [JsonIgnore]public int? ObjectId => this.AdditionalActivityId;
}

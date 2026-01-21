namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveSpbsBookRecordEscapeCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    [JsonIgnore]public int? SpbsBookRecordId { get; init; }
    [JsonIgnore]public int? OrderNum { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SpbsBookRecordEscape);
    [JsonIgnore]public int? ObjectId => this.OrderNum;
}

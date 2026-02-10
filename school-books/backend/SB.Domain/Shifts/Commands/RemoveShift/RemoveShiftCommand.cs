namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveShiftCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? ShiftId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Shift);
    [JsonIgnore]public int? ObjectId => this.ShiftId;
}

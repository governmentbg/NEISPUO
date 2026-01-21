namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveOffDayCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? OffDayId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(OffDay);
    [JsonIgnore]public int? ObjectId => this.OffDayId;
}

namespace SB.Domain;

using MediatR;
using System.Text.Json.Serialization;

public record RemoveParentMeetingCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? ParentMeetingId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(ParentMeeting);
    [JsonIgnore]public int? ObjectId => this.ParentMeetingId;
}

namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record CreateSpbsBookRecordEscapeCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? SpbsBookRecordId { get; init; }

    public DateTime? EscapeDate { get; init; }
    public string? EscapeTime { get; init; }
    public DateTime? PoliceNotificationDate { get; init; }
    public string? PoliceNotificationTime { get; init; }
    public string? PoliceLetterNumber { get; init; }
    public DateTime? PoliceLetterDate { get; init; }
    public DateTime? ReturnDate { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SpbsBookRecordEscape);
    [JsonIgnore]public virtual int? ObjectId => null;
}

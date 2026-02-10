namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record RemoveAttendancesCommand : IRequest, IAuditedUpdateMultipleCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int[]? AttendanceIds { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Attendance);
    [JsonIgnore]public virtual int? ObjectId => null;
    [JsonIgnore]public int[] ObjectIds => this.AttendanceIds ?? Array.Empty<int>();
}

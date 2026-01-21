namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateAttendancesCommand : IAuditedCreateMultipleCommand
{
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public DateTime? Date { get; init; }

    public CreateAttendancesCommandAttendance[]? Attendances { get; init; }

    public int? ExcusedReasonId { get; init; }
    public string? ExcusedReasonComment { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Attendance);
    [JsonIgnore]public virtual int? ObjectId => null;
}

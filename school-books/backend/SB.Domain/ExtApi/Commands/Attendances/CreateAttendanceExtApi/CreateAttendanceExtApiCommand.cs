namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateAttendanceExtApiCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public int? PersonId { get; init; }
    public DateTime? Date { get; init; }
    public AttendanceType? Type { get; init; }
    public int? ExcusedReasonId { get; init; }
    public string? ExcusedReasonComment { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Attendance);
    [JsonIgnore]public virtual int? ObjectId => null;
}

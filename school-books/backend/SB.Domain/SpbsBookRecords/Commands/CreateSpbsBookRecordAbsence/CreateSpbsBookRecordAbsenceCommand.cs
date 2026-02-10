namespace SB.Domain;

using MediatR;
using System;
using System.Text.Json.Serialization;

public record CreateSpbsBookRecordAbsenceCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? SpbsBookRecordId { get; init; }

    public DateTime? AbsenceDate { get; init; }
    public string? AbsenceReason { get; init; }

    [JsonIgnore]public string ObjectName => nameof(SpbsBookRecordAbsence);
    [JsonIgnore]public virtual int? ObjectId => null;
}

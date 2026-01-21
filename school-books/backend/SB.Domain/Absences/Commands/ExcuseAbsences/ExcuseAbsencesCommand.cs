namespace SB.Domain;

using MediatR;
using System;
using System.Linq;
using System.Text.Json.Serialization;

public record ExcuseAbsencesCommand : IRequest, IAuditedUpdateMultipleCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public ExcuseAbsencesCommandAbsence[]? Absences { get; init; }

    public int? ExcusedReasonId { get; init; }
    public string? ExcusedReasonComment { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Absence);
    [JsonIgnore]public virtual int? ObjectId => null;
    [JsonIgnore]public int[] ObjectIds =>
        this.Absences
            ?.Select(x => x.AbsenceId!.Value)
            ?.ToArray()
        ?? Array.Empty<int>();
}

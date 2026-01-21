namespace SB.Domain;

using System.Text.Json.Serialization;

public record CreateAbsenceCommand : IAuditedCreateMultipleCommand
{
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? ClassBookId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }

    public CreateAbsenceCommandAbsence[]? Absences { get; init; }

    public int? ExcusedReasonId { get; init; }
    public string? ExcusedReasonComment { get; init; }

    [JsonIgnore]public string ObjectName => nameof(Absence);
    [JsonIgnore]public virtual int? ObjectId => null;
}

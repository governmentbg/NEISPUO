namespace SB.Domain;

using System;
using System.Text.Json.Serialization;

public record CreateTeacherAbsenceCommand : IAuditedCreateCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? TeacherAbsenceId { get; init; }

    public int? TeacherPersonId { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Reason { get; init; }
    public CreateTeacherAbsenceCommandHour[]? Hours  { get; init; }

    [JsonIgnore]public string ObjectName => nameof(TeacherAbsence);
    [JsonIgnore]public int? ObjectId => this.TeacherAbsenceId;
}

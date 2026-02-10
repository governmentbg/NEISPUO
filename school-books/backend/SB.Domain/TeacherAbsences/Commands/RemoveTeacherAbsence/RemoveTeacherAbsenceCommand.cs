namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record RemoveTeacherAbsenceCommand : IRequest, IAuditedCommand
{
    [JsonIgnore]public int? SchoolYear { get; init; }
    [JsonIgnore]public int? InstId { get; init; }
    [JsonIgnore]public int? SysUserId { get; init; }
    [JsonIgnore]public int? TeacherAbsenceId { get; init; }

    [JsonIgnore]public string ObjectName => nameof(TeacherAbsence);
    [JsonIgnore]public int? ObjectId => this.TeacherAbsenceId;
}

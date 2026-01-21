namespace SB.Domain;

using System.Text.Json.Serialization;
using MediatR;

public record CreateUpdateStudentSettingsCommand : IRequest<int>
{
    [JsonIgnore] public int? SysUserId { get; init; }
    [JsonIgnore] public int? UserPersonId { get; init; }

    public bool? AllowGradeEmails { get; init; }

    public bool? AllowAbsenceEmails { get; init; }

    public bool? AllowRemarkEmails { get; init; }

    public bool? AllowMessageEmails { get; init; }

    public bool? AllowGradeNotifications { get; init; }

    public bool? AllowAbsenceNotifications { get; init; }

    public bool? AllowRemarkNotifications { get; init; }

    public bool? AllowMessageNotifications { get; init; }

    [JsonIgnore] public string ObjectName => nameof(SchoolYearSettings);
    [JsonIgnore] public virtual int? ObjectId => null;
}

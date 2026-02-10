namespace SB.Domain;

public partial interface IStudentSettingsQueryRepository
{
    public record GetVO(
        bool AllowGradeEmails = false,
        bool AllowAbsenceEmails = false,
        bool AllowRemarkEmails = false,
        bool AllowMessageEmails = false,
        bool AllowGradeNotifications = false,
        bool AllowAbsenceNotifications = false,
        bool AllowRemarkNotifications = false,
        bool AllowMessageNotifications = false);
}

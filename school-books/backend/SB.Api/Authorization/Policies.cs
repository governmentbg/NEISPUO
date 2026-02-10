namespace SB.Api;

public static class Policies
{
    public const string InstitutionAccess = nameof(InstitutionAccess);
    public const string ReportAccess = nameof(ReportAccess);
    public const string InstitutionAdminAccess = nameof(InstitutionAdminAccess);
    public const string ClassBookAccess = nameof(ClassBookAccess);
    public const string ClassBookAdminAccess = nameof(ClassBookAdminAccess);
    public const string StudentInfoClassBookAccess = nameof(StudentInfoClassBookAccess);
    public const string CurriculumAccess = nameof(CurriculumAccess);
    public const string ScheduleLessonAccess = nameof(ScheduleLessonAccess);
    public const string SupportAccess = nameof(SupportAccess);
    public const string AttendanceDateAccess = nameof(AttendanceDateAccess);
    public const string HisMedicalNoticeAccess = nameof(HisMedicalNoticeAccess);

    public const string StudentAccess = nameof(StudentAccess);
    public const string StudentClassBookAccess = nameof(StudentClassBookAccess);
    public const string StudentMedicalNoticesAccess = nameof(StudentMedicalNoticesAccess);
    public const string ConversationAccess = nameof(ConversationAccess);

    public const string AuthenticatedAccess = nameof(AuthenticatedAccess);
    public const string DenyAll = nameof(DenyAll);
}

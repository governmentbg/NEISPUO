namespace SB.ExtApi;

public static class AuthorizationConstants
{
    public const int ExtSystemTypeSchoolBooks = 1; // Електронни дневници
    public const int ExtSystemTypeExternalIntegration = 3; // Външна интеграция
    public const int ExtSystemTypeSchedule = 4; // aSc - Седмично разписание

    public const string ScheduleProviderAdditionalAccess = nameof(ScheduleProviderAdditionalAccess);
}

namespace SB.Domain;
public partial interface IStudentClassBooksQueryRepository
{
    public record GetAttendanceMonthStatsVO(
        int Year,
        int Month,
        int PresencesCount,
        int UnexcusedAbsencesCount,
        int ExcusedAbsencesCount);
}

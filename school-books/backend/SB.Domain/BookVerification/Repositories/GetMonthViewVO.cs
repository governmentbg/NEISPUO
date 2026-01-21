namespace SB.Domain;

public partial interface IBookVerificationQueryRepository
{
    public record GetMonthViewVO(
        int Day,
        bool HasData,
        bool IsOffDay,
        int TakenPercentage,
        int VerifiedPercentage);
}

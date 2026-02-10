namespace SB.Domain;

public partial interface IBookVerificationQueryRepository
{
    public record GetYearViewVO(
        int Year,
        int Month,
        bool HasData,
        int TakenPercentage,
        int VerifiedPercentage);
}

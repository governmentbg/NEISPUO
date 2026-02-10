namespace SB.Domain;
public partial interface IRemarksQueryRepository
{
    public record GetAllForClassBookVO(
        int PersonId,
        int BadRemarksCount,
        int GoodRemarksCount);
}

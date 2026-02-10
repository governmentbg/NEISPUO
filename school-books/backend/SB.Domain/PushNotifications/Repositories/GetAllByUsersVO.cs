namespace SB.Domain;

public partial interface IPushSubscriptionsQueryRepository
{
    public record GetAllByUsersVO(
        string Endpoint,
        string P256dh,
        string Auth);
}

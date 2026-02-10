namespace SB.Domain;

using Lib.Net.Http.WebPush;
using System.Threading;
using System.Threading.Tasks;

class PushSubscriptionsService : IPushSubscriptionsService
{
    private IUnitOfWork unitOfWork;
    private IPushSubscriptionsQueryRepository pushSubscriptionsQueryRepository;
    private IPushSubscriptionsAggregateRepository pushSubscriptionsAggregateRepository;

    public PushSubscriptionsService(
        IUnitOfWork unitOfWork,
        IPushSubscriptionsQueryRepository pushSubscriptionsQueryRepository,
        IPushSubscriptionsAggregateRepository pushSubscriptionsAggregateRepository)
    {
        this.unitOfWork = unitOfWork;
        this.pushSubscriptionsQueryRepository = pushSubscriptionsQueryRepository;
        this.pushSubscriptionsAggregateRepository = pushSubscriptionsAggregateRepository;
    }

    public async Task AddOrUpdateAsync(PushSubscription subscription, int sysUserId, CancellationToken ct)
    {
        var p256DH = subscription.GetKey(PushEncryptionKeyName.P256DH);
        var auth = subscription.GetKey(PushEncryptionKeyName.Auth);

        var existingSubscription = await this.pushSubscriptionsAggregateRepository
            .FindOrDefaultAsync(subscription.Endpoint, ct); // Query by Endpoint instead of SysUserId

        if (existingSubscription != null)
        {
            if (existingSubscription.P256dh != p256DH || existingSubscription.Auth != auth)
            {
                existingSubscription.UpdateData(p256DH, auth);
            }
        }
        else
        {
            // Add a new subscription for this user
            await this.pushSubscriptionsAggregateRepository.AddAsync(
                new UserPushSubscription(sysUserId, subscription.Endpoint, p256DH, auth),
                false,
                ct);
        }

        await this.unitOfWork.SaveAsync(ct);
    }
}

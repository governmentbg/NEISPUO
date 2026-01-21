namespace SB.Domain;

using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

class PushNotificationsService : IPushNotificationsService
{
    private IUnitOfWork unitOfWork;
    private IPushSubscriptionsAggregateRepository pushSubscriptionsAggregateRepository;
    private PushServiceClient pushServiceClient;

    public PushNotificationsService(
        IUnitOfWork unitOfWork,
        IPushSubscriptionsAggregateRepository pushSubscriptionsAggregateRepository,
        PushServiceClient pushServiceClient,
        IOptions<DomainOptions> optionsAccessor)
    {
        this.unitOfWork = unitOfWork;
        this.pushSubscriptionsAggregateRepository = pushSubscriptionsAggregateRepository;
        this.pushServiceClient = pushServiceClient;
        this.pushServiceClient.DefaultAuthentication = new VapidAuthentication(optionsAccessor.Value.VAPIDPublicKey, optionsAccessor.Value.VAPIDPrivateKey)
        {
            // TODO use settings or don't set the Subject
            Subject = "https://dnevnik.mon.bg"
        };

    }

    public async Task SendNotifications(string title, string body, int sysUserId, CancellationToken ct)
    {
        PushMessage pushMessage = new AngularPushNotification
        {
            Title = title,
            Body = body
        }.ToPushMessage();

        var subscriptions = await this.pushSubscriptionsAggregateRepository.FindAllBySysUserIdAsync(sysUserId, ct);

        foreach (var subscription in subscriptions)
        {
            var clientSubscription = new PushSubscription
            {
                Endpoint = subscription.Endpoint
            };
            clientSubscription.SetKey(PushEncryptionKeyName.P256DH, subscription.P256dh);
            clientSubscription.SetKey(PushEncryptionKeyName.Auth, subscription.Auth);

            try
            {
                await this.pushServiceClient.RequestPushMessageDeliveryAsync(clientSubscription, pushMessage, ct);
            }
            catch (PushServiceClientException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Gone || ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                this.pushSubscriptionsAggregateRepository.Remove(subscription);
                await this.unitOfWork.SaveAsync(ct);
            }
            catch (Exception ex)
            {
                // Handle other errors (log, retry, etc.)
                Console.WriteLine($"Failed to send notification to {subscription.Endpoint}: {ex.Message}");
            }
        }
    }
}

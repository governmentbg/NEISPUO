namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IPushSubscriptionsService
{
    Task AddOrUpdateAsync(Lib.Net.Http.WebPush.PushSubscription subscription, int sysUserId, CancellationToken ct);
}

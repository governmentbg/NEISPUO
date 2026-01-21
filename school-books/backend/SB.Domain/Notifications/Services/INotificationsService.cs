namespace SB.Domain;

using Newtonsoft.Json.Linq;
using System.Threading;
using System.Threading.Tasks;

public interface INotificationsService
{
    Task TryPostNotificationsAsync(string eventType, int studentPersonId, JObject jObject, CancellationToken ct);
    Task TryPostNewMessageNotificationsAsync(int conversationId, int sysUserId, CancellationToken ct);
}

namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;

public interface IPushNotificationsService
{
    Task SendNotifications(string title, string body, int sysUserId, CancellationToken ct);
}

namespace SB.Domain;

using System.Threading;
using System.Threading.Tasks;
using SendGrid;

public interface IEmailSenderService
{
    Task SendEmailAsync(SendGridClient client, NotificationQueueMessage payload, CancellationToken ct);
}

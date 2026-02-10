namespace SB.JobHost;

using Microsoft.Extensions.Options;
using SB.Domain;
using SendGrid;

public class NotificationJob : QueueJob<NotificationQueueMessage>
{
    private readonly string sendGridApiKey;
    private readonly IEmailSenderService emailSenderService;
    private readonly IPushNotificationsService pushNotificationsService;

    public NotificationJob(
        IServiceScopeFactory serviceScopeFactory,
        IEmailSenderService emailSenderService,
        IPushNotificationsService pushNotificationsService,
        ILogger<NotificationJob> logger,
        IOptions<JobHostOptions> optionsAccessor)
        : base(
            QueueMessageType.Notification,
            serviceScopeFactory,
            logger,
            optionsAccessor.Value.EmailJobOptions)
    {
        this.emailSenderService = emailSenderService;
        this.pushNotificationsService = pushNotificationsService;
        var options = optionsAccessor.Value.EmailJobOptions;
        this.sendGridApiKey = options.SendGridApiKey;
    }

    protected override async Task<(QueueJobProcessingResult, string?)> HandleMessageAsync(
        NotificationQueueMessage payload,
        CancellationToken ct)
    {
        if (payload.NotificationType == NotificationType.Email)
        {
            try
            {
                var client = new SendGridClient(this.sendGridApiKey);
                await this.emailSenderService.SendEmailAsync(client, payload, ct);
            }
            catch when (ct.IsCancellationRequested)
            {
                throw new QueueJobRetryErrorException("Cancelled");
            }
        }
        else
        {
            await this.pushNotificationsService.SendNotifications(payload.PushMessageTitle!, payload.PushMessageBody!, payload.RecipientSysUserId!.Value, ct);
        }
        

        return (QueueJobProcessingResult.Success, null);
    }
}

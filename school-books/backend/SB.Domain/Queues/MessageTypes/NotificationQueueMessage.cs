namespace SB.Domain;
using Newtonsoft.Json.Linq;

public record NotificationQueueMessage(
    NotificationType NotificationType,
    int? RecipientSysUserId,
    string? RecipientEmail,
    string? MailTemplateName,
    string? PushMessageTitle,
    string? PushMessageBody,
    JObject? Context = null
);

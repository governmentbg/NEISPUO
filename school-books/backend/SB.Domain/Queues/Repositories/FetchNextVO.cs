namespace SB.Domain;

using Microsoft.EntityFrameworkCore;

public partial interface IQueueMessagesRepository
{
    [Keyless]
    public record FetchNextVO(
        QueueMessageType Type,
        long DueDateUnixTimeMs,
        int QueueMessageId,
        string Payload,
        int FailedAttempts);
}

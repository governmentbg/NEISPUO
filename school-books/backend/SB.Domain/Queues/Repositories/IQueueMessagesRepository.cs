namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial interface IQueueMessagesRepository : IRepository
{
    Task InsertQueueMessagesAsync(
        QueueMessageType type,
        string[] payloads,
        long? dueDateUnixTimeMs,
        string? tag,
        string? key,
        CancellationToken ct);

    Task<IEnumerable<FetchNextVO>> FetchNextAsync(
        QueueMessageType type,
        int nextCount,
        TimeSpan failedAttemptTimeout,
        CancellationToken ct);

    Task MakePendingAsync(
        QueueMessageType type,
        long startDateUnixTimeMilliseconds,
        long endDateUnixTimeMilliseconds,
        int[] queueMessageIds,
        CancellationToken ct);

    Task SetStatusProcessedAsync(
        QueueMessageType type,
        long dueDateUnixTimeMs,
        int queueMessageId,
        CancellationToken ct);

    Task SetErrorAsync(
        QueueMessageType type,
        long dueDateUnixTimeMs,
        int queueMessageId,
        int maxFailedAttempts,
        string error,
        CancellationToken ct);

    Task<IEnumerable<QueueMessage>> CancelMessagesAsync(
        QueueMessageType type,
        string tag,
        CancellationToken ct);
}

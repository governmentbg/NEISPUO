namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IQueueMessagesService
{
    Task PostMessageAndSaveAsync<T>(
        T payload,
        CancellationToken ct) where T : notnull;

    Task PostMessagesAndSaveAsync<T>(
        T[] payloads,
        CancellationToken ct) where T : notnull;

    Task PostMessagesAndSaveAsync<T>(
        T[] payloads,
        string tag,
        CancellationToken ct) where T : notnull;

    Task PostMessageAndSaveAsync<T>(
        T payload,
        DateTime dueDate,
        CancellationToken ct) where T : notnull;

    Task PostMessageAndSaveAsync<T>(
        T payload,
        string tag,
        CancellationToken ct) where T : notnull;

    Task PostMessageAndSaveAsync<T>(
        T payload,
        DateTime? dueDate,
        string? tag,
        string? key,
        CancellationToken ct) where T : notnull;

    Task PostMessagesAndSaveAsync<T>(
        T[] payloads,
        long? dueDateUnixTimeMs,
        string? tag,
        string? key,
        CancellationToken ct) where T : notnull;

    Task PostMessagesRawAndSaveAsync(
        QueueMessageType type,
        string[] payloads,
        long? dueDateUnixTimeMs,
        string? tag,
        string? key,
        CancellationToken ct);

    Task<IEnumerable<(T, DateTime)>> CancelMessagesAndSaveAsync<T>(
        string tag,
        CancellationToken ct) where T : notnull;
}

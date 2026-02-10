namespace SB.Domain;

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

internal class QueueMessagesService : IQueueMessagesService
{
    private IQueueMessagesRepository queueMessagesRepository;

    public QueueMessagesService(IQueueMessagesRepository queueMessagesRepository)
    {
        this.queueMessagesRepository = queueMessagesRepository;
    }

    public Task PostMessageAndSaveAsync<T>(
        T payload,
        CancellationToken ct) where T : notnull
    {
        return this.PostMessagesAndSaveAsync(
            payloads: new[] { payload },
            dueDateUnixTimeMs: null,
            tag: null,
            key: null,
            ct: ct);
    }

    public Task PostMessagesAndSaveAsync<T>(
        T[] payloads,
        CancellationToken ct) where T : notnull
    {
        return this.PostMessagesAndSaveAsync(
            payloads: payloads,
            dueDateUnixTimeMs: null,
            tag: null,
            key: null,
            ct: ct);
    }

    public Task PostMessagesAndSaveAsync<T>(
        T[] payloads,
        string tag,
        CancellationToken ct) where T : notnull
    {
        return this.PostMessagesAndSaveAsync(
            payloads: payloads,
            dueDateUnixTimeMs: null,
            tag: tag,
            key: null,
            ct: ct);
    }

    public Task PostMessageAndSaveAsync<T>(
        T payload,
        DateTime dueDate,
        CancellationToken ct) where T : notnull
    {
        return this.PostMessagesAndSaveAsync(
            payloads: new[] { payload },
            dueDateUnixTimeMs: ((DateTimeOffset?)dueDate)?.ToUnixTimeMilliseconds(),
            tag: null,
            key: null,
            ct: ct);
    }

    public Task PostMessageAndSaveAsync<T>(
        T payload,
        string tag,
        CancellationToken ct) where T : notnull
    {
        return this.PostMessagesAndSaveAsync(
            payloads: new[] { payload },
            dueDateUnixTimeMs: null,
            tag: tag,
            key: null,
            ct: ct);
    }

    public Task PostMessageAndSaveAsync<T>(
        T payload,
        DateTime? dueDate,
        string? tag,
        string? key,
        CancellationToken ct) where T : notnull
    {
        return this.PostMessagesAndSaveAsync(
            payloads: new[] { payload },
            dueDateUnixTimeMs: ((DateTimeOffset?)dueDate)?.ToUnixTimeMilliseconds(),
            tag: tag,
            key: key,
            ct: ct);
    }

    public async Task PostMessagesAndSaveAsync<T>(
        T[] payloads,
        long? dueDateUnixTimeMs,
        string? tag,
        string? key,
        CancellationToken ct) where T : notnull
    {
        await this.PostMessagesRawAndSaveAsync(
            GetType<T>(),
            payloads
                .Select(payload =>
                    JsonConvert.SerializeObject(
                        payload))
                .ToArray(),
            dueDateUnixTimeMs,
            tag,
            key,
            ct);
    }

    public async Task PostMessagesRawAndSaveAsync(
        QueueMessageType type,
        string[] payloads,
        long? dueDateUnixTimeMs,
        string? tag,
        string? key,
        CancellationToken ct)
    {
        await this.queueMessagesRepository.InsertQueueMessagesAsync(
            type,
            payloads,
            dueDateUnixTimeMs,
            tag,
            key,
            ct);
    }

    public async Task<IEnumerable<(T, DateTime)>> CancelMessagesAndSaveAsync<T>(
        string tag,
        CancellationToken ct)
        where T : notnull
    {
        var cancelledMessages =
            await this.queueMessagesRepository.CancelMessagesAsync(GetType<T>(), tag, ct);

        return cancelledMessages
            .Select(m => (
                payload:
                    JsonConvert.DeserializeObject<T>(m.Payload)
                    ?? throw new Exception("Payload should not be null"),
                dueDate: DateTimeOffset.FromUnixTimeMilliseconds(m.DueDateUnixTimeMs).UtcDateTime
            ))
            .ToList();
    }

    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All)
    };

    private static QueueMessageType GetType<T>() where T : notnull
        => typeof(T) switch
        {
            var t when t == typeof(PrintHtmlQueueMessage) => QueueMessageType.PrintHtml,
            var t when t == typeof(PrintPdfQueueMessage) => QueueMessageType.PrintPdf,
            var t when t == typeof(NotificationQueueMessage) => QueueMessageType.Notification,
            _ => throw new Exception("Unknown payload type.")
        };
}

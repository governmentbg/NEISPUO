namespace SB.JobHost;

using SB.Domain;
using static SB.Domain.IQueueMessagesRepository;
using System;
using Newtonsoft.Json;
#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

public abstract class QueueJob<TMessage> : PeriodicBackgroundService
{
    private QueueMessageType type;
    private ILogger logger;
    private IServiceScope scope;
    private IQueueMessagesRepository queueMessagesRepository;
    private Queue<FetchNextVO> queueMessagesQueue = new();

    private int batchSize;
    private int maxFailedAttempts;
    private TimeSpan failedAttemptTimeout;

    protected QueueJob(
        QueueMessageType type,
        IServiceScopeFactory scopeFactory,
        ILogger logger,
        QueueJobOptions options)
        : base (TimeSpan.FromSeconds(options.BackoffPeriodInSeconds))
    {
        this.type = type;
        this.logger = logger;
        this.scope = scopeFactory.CreateScope();
        this.queueMessagesRepository =
            this.scope.ServiceProvider
            .GetRequiredService<IQueueMessagesRepository>();
        this.batchSize = options.BatchSize;
        this.maxFailedAttempts = options.MaxFailedAttempts;
        this.failedAttemptTimeout =
            TimeSpan.FromMinutes(options.FailedAttemptTimeoutInMinutes);
    }

    public override void Dispose()
    {
        this.scope.Dispose();
        base.Dispose();
    }

    protected ILogger Logger => this.logger;

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);
    }

    protected override async Task<bool> ExecuteAsync(
        CancellationToken stoppingToken,
        CancellationToken stopToken)
    {
        try
        {
            var queueMessages = await this.queueMessagesRepository.FetchNextAsync(
                this.type,
                this.batchSize,
                this.failedAttemptTimeout,
                stoppingToken);

            var total = queueMessages.Count();
            if (total == 0)
            {
                return true;
            }

            foreach (var queueMessage in queueMessages)
            {
                this.queueMessagesQueue.Enqueue(queueMessage);
            }

                await this.HandleAsync(stoppingToken);

            if (this.queueMessagesQueue.Count > 0)
            {
                await this.queueMessagesRepository.MakePendingAsync(
                    this.type,
                    this.queueMessagesQueue.Min(m => m.DueDateUnixTimeMs),
                    this.queueMessagesQueue.Max(m => m.DueDateUnixTimeMs),
                    this.queueMessagesQueue.Select(e => e.QueueMessageId).ToArray(),
                    // try and finish the operation, no cancellation
                    default);
            }

            return false;
        }
        catch (Exception ex)
        {
            this.logger.Log(LogLevel.Error, ex, "Job execution threw an exception {@ex}", ex);
            // backoff in case of unexpected error
            // so that we don't flood the sql server
            return true;
        }
    }

    private async Task HandleAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested
            && this.queueMessagesQueue.TryDequeue(out var message))
        {
            try
            {
                TMessage payload =
                    JsonConvert.DeserializeObject<TMessage>(message.Payload)
                    ?? throw new Exception("Payload should not be null");

                QueueJobProcessingResult result;
                string? error;

                try
                {
                    (result, error) = await this.HandleMessageAsync(payload, ct);
                }
                catch (QueueJobRetryErrorException ex)
                {
                    result = QueueJobProcessingResult.RetryError;
                    error = ex.Error;
                }

                if (result == QueueJobProcessingResult.Success)
                {
                    await this.queueMessagesRepository.SetStatusProcessedAsync(
                        message.Type,
                        message.DueDateUnixTimeMs,
                        message.QueueMessageId,
                        // not passing cancellationToken as this
                        // operation must succeed even when cancelled
                        default
                    );
                }
                else if (result == QueueJobProcessingResult.RetryError)
                {
                    await this.queueMessagesRepository.SetErrorAsync(
                        message.Type,
                        message.DueDateUnixTimeMs,
                        message.QueueMessageId,
                        this.maxFailedAttempts,
                        error ?? "",
                        // not passing cancellationToken as this
                        // operation must succeed even when cancelled
                        default
                    );
                }
                else
                {
                    throw new Exception("Unknown QueueJobProcessingResult");
                }
            }
            catch (Exception ex)
            {
                await this.queueMessagesRepository.SetErrorAsync(
                    message.Type,
                    message.DueDateUnixTimeMs,
                    message.QueueMessageId,
                    0, // skip retrying, this a non-recoverable error
                    ex.ToString(),
                    default
                );

                this.Logger.Log(LogLevel.Error, ex, "QueueMessage handling threw an exception {@ex}", ex);
            }
        }
    }

    protected abstract Task<(QueueJobProcessingResult result, string? error)> HandleMessageAsync(
        TMessage payload,
        CancellationToken ct);
}

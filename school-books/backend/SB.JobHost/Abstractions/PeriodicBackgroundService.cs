namespace SB.JobHost;

using System.Diagnostics;

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize

public abstract class PeriodicBackgroundService : IHostedService, IDisposable
{
    private Task? executingTask;
    private TimeSpan period;

    private readonly CancellationTokenSource stoppingCts = new();
    private readonly CancellationTokenSource stopCts = new();

    protected PeriodicBackgroundService(TimeSpan period)
    {
        this.period = period;
    }

    protected abstract Task<bool> ExecuteAsync(
        CancellationToken stoppingToken,
        CancellationToken stopToken);

    public virtual Task StartAsync(CancellationToken cancellationToken)
    {
        if (this.period == TimeSpan.Zero)
        {
            return Task.CompletedTask;
        }

        this.executingTask =
            Task.Run(
                async () =>
                {
                    Stopwatch sw = new();
                    do
                    {
                        sw.Restart();

                        bool backoff = await this.ExecuteAsync(
                            this.stoppingCts.Token,
                            this.stopCts.Token);

                        sw.Stop();

                        if (backoff && sw.Elapsed < this.period)
                        {
                            try
                            {
                                await Task.Delay(
                                    this.period - sw.Elapsed,
                                    this.stoppingCts.Token);
                            }
                            catch
                            {
                            }
                        }
                    }
                    while (!this.stoppingCts.Token.IsCancellationRequested);
                },
                cancellationToken);

        return Task.CompletedTask;
    }

    public virtual async Task StopAsync(CancellationToken cancellationToken)
    {
        // Stop called without start
        if (this.executingTask == null)
        {
            return;
        }

        cancellationToken.Register(() => this.stopCts.Cancel());

        try
        {
            // Signal cancellation to the executing method
            this.stoppingCts.Cancel();
        }
        finally
        {
            await this.executingTask;
        }
    }

    public virtual void Dispose()
    {
        this.stoppingCts.Cancel();
        this.stopCts.Cancel();
    }
}

namespace MON.Services.Hangfire
{
    using global::Hangfire;
    using System;
    using System.Threading.Tasks;

    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    public class AspSessionJob : IHangfireJob
    {
        public async Task Run(global::Hangfire.IJobCancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await RunAtTimeOf(DateTime.UtcNow);
        }

        public async Task RunAtTimeOf(DateTime now)
        {
            // Do some work
           await Task.CompletedTask;
        }
    }
}

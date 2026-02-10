namespace MON.API
{
    using Hangfire;
    using MON.Services.Hangfire;
    using System;

    public class HangfireJobScheduler
    {
        public static void ScheduleRecurringJobs(IServiceProvider serviceProvider)
        {
            // Every 5 minutes
            RecurringJob.RemoveIfExists(nameof(AspSessionJob));
            RecurringJob.AddOrUpdate<AspSessionJob>(nameof(AspSessionJob),
                job => job.Run(JobCancellationToken.Null),
                $"0 0 * * *", TimeZoneInfo.Utc);
        }
    }
}

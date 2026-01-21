namespace SB.JobHost;

using System.ComponentModel.DataAnnotations;

public class QueueJobOptions
{
    [Required]
    public int BackoffPeriodInSeconds { get; set; }

    [Required]
    public int BatchSize { get; set; }

    [Required]
    public int JobInstances { get; set; }

    [Required]
    public int FailedAttemptTimeoutInMinutes { get; set; }

    [Required]
    public int MaxFailedAttempts { get; set; }
}

namespace SB.JobHost;

using System.ComponentModel.DataAnnotations;

public class MedicalNoticeJobOptions
{
    [Required]
    public bool IsEnabled { get; init; }

    [Required]
    public TimeSpan BackoffPeriod { get; set; }

    [Required]
    public int BatchSize { get; set; }
}

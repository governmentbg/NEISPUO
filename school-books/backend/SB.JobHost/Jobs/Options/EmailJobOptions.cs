namespace SB.JobHost;

using System.ComponentModel.DataAnnotations;

public class EmailJobOptions : QueueJobOptions
{
    [Required]
    public required string SendGridApiKey { get; init; }
}

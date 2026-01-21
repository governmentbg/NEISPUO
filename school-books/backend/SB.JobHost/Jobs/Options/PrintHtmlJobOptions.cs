namespace SB.JobHost;

using System.ComponentModel.DataAnnotations;

public class PrintHtmlJobOptions : QueueJobOptions
{
    [Required]
    public int MemoryPoolSizeMB { get; set; }
}

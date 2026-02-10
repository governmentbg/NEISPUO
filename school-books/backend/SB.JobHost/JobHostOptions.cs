namespace SB.JobHost;

using System.ComponentModel.DataAnnotations;

public class JobHostOptions
{
    [Required]
    public TimeSpan ShutdownTimeout { get; init; }

    [Required]
    public required PrintHtmlJobOptions PrintHtmlJobOptions { get; init; }

    [Required]
    public required PrintPdfJobOptions PrintPdfJobOptions { get; init; }

    [Required]
    public required MedicalNoticeJobOptions MedicalNoticeJobOptions { get; init; }

    [Required]
    public required EmailJobOptions EmailJobOptions { get; init; }
}

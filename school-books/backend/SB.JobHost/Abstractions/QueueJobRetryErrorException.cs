namespace SB.JobHost;

public class QueueJobRetryErrorException : Exception
{
    public QueueJobRetryErrorException(string error) : base("Retry error: " + error)
    {
        this.Error = error;
    }

    public string Error { get; init; }
}

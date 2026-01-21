namespace SB.JobHost;

public class BatchCounter
{
    private int successes;
    private int failures;

    public int Successes => this.successes;
    public int Failures => this.failures;

    public void CountSuccess()
    {
        Interlocked.Increment(ref this.successes);
    }

    public void CountFailure()
    {
        Interlocked.Increment(ref this.failures);
    }
}

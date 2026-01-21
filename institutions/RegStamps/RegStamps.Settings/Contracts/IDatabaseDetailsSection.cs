namespace RegStamps.Settings.Contracts
{
    public interface IDatabaseDetailsSection
    {
        string ConnectionString { get; set; }
        int CommandTimeout { get; set; }
    }
}

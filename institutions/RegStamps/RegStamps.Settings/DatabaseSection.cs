namespace RegStamps.Settings
{
    using Contracts;

    public class DatabaseSection : IDatabaseSection
    {
        public IDatabaseDetailsSection DataStampsDatabase { get; set; } = new DatabaseDetailsSection();
    }
}

namespace MonProjects.Configurations
{
    using Contracts;

    public class DatabaseSection : IDatabaseSection
    {
        public IDatabaseDetailsSection Neispuo { get; set; } = new DatabaseDetailsSection();
    }
}

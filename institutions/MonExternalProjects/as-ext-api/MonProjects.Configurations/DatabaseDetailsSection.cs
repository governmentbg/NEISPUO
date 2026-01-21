namespace MonProjects.Configurations
{
    using Contracts;
    public class DatabaseDetailsSection : IDatabaseDetailsSection
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
    }
}

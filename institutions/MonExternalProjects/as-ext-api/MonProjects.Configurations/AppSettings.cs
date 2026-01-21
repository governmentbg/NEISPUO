namespace MonProjects.Configurations
{
    using Contracts;

    public class AppSettings
    {
        public IDatabaseSection Database { get; private set; } = new DatabaseSection();
    }
}

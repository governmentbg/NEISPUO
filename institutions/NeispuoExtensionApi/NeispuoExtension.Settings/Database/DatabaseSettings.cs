namespace NeispuoExtension.Settings.Database
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public int CommandTimeout { get; set; }

        public string ConnectionString { get; set; }
    }
}

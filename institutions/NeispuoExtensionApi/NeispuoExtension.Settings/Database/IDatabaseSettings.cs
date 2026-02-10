namespace NeispuoExtension.Settings.Database
{
    public interface IDatabaseSettings
    {
        public int CommandTimeout { get; set; }

        public string ConnectionString { get; set; }
    }
}

namespace RegStamps.Settings
{
    using Contracts;

    public class AppSettings
    {
        public IDatabaseSection Database { get; private set; } = new DatabaseSection();

        public INeispuoSection Neispuo { get; set; } = new NeispuoSection();
    }
}

namespace NeispuoExtension.Settings
{
    using Authentication;
    using NeispuoExtension.Settings.Database;

    public class ApplicationSettings
    {
        public string BaseErrorLogDirectory { get; set; }

        public IAuthenticationSettings Authentication { get; set; } = new AuthenticationSettings();

        public IDatabaseSettings NeispuoDatabase { get; set; } = new DatabaseSettings();
    }
}

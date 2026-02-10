namespace NeispuoExtension.Settings.Authentication
{
    public interface IAuthenticationSettings
    {
        public string Audience { get; set; }

        public string Authority { get; set; }
    }
}

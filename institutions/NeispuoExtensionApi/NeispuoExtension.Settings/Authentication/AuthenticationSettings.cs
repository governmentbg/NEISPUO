namespace NeispuoExtension.Settings.Authentication
{
    public class AuthenticationSettings : IAuthenticationSettings
    {
        public string Audience { get; set; }

        public string Authority { get; set; }
    }
}

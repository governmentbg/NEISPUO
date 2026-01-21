namespace Helpdesk.API.Identity
{
    public class AuthenticationConfig
    {
        public string[] AllowedCorsOrigins { get; set; }
        public string IdentityServerUrl { get; set; }

        public string ApiName { get; set; }
    }
}

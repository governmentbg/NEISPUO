namespace Kontrax.Regix.Core
{
    public enum SearchLocationType { Computer, File };

    public class RegixConfig
    {
        public SearchLocationType SearchLocation { get; set; }
        public string FileLocation { get; set; }
        public string Password { get; set; }
        public string Thumbprint { get; set; }
        public string Url { get; set; }
    }
}

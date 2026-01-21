namespace RegStamps.Settings
{
    using Contracts;

    public class NeispuoSection : INeispuoSection
    {
        public string BaseUrl { get; set; }
        public int CommandTimeout { get; set; }
        public INeispuoCertificateSection Certificate { get; set; } = new NeispuoCertificateSection();
    }
}

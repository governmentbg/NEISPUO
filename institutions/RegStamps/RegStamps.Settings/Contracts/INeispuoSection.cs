namespace RegStamps.Settings.Contracts
{
    public interface INeispuoSection
    {
        public string BaseUrl { get; set; }

        public int CommandTimeout { get; set; }

        public INeispuoCertificateSection Certificate { get; set; }
    }
}

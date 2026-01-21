namespace RegStamps.Settings.Contracts
{
    public interface INeispuoCertificateSection
    {
        public string CertificatePath { get; set; }

        public string PrivateKeyPath { get; set; }
    }
}

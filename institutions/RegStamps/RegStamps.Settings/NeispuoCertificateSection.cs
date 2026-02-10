using RegStamps.Settings.Contracts;

namespace RegStamps.Settings
{
    public class NeispuoCertificateSection : INeispuoCertificateSection
    {
        public string CertificatePath { get; set; }
        public string PrivateKeyPath { get; set; }
    }
}

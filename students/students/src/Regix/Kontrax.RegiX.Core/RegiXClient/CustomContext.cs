using System.Security.Cryptography.X509Certificates;

namespace Kontrax.RegiX.Core.TestStandard
{
    public class CustomContext
    {
        public X509Certificate2 Certificate { get; }
        public string EAuthToken { get; }

        public CustomContext()
        { }

        public CustomContext(
            X509Certificate2 certificate,
            string eAuthToken)
        {
            Certificate = certificate;
            EAuthToken = eAuthToken;
        }
    }
}

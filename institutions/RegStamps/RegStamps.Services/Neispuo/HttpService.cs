namespace RegStamps.Services.Neispuo
{
    using Microsoft.Extensions.Options;

    using System.Security.Authentication;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;

    using Settings;
    using Settings.Contracts;

    public class HttpService : IHttpService
    {
        private readonly INeispuoSection neispuoSettings;

        public HttpService(IOptions<AppSettings> options)
        {
            this.neispuoSettings = options.Value.Neispuo;
        }

        public async Task<HttpResponseMessage> GetAsync(string path)
            => await this.SendAsync(HttpMethod.Get, path);

        private async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string path)
        {
            try
            {
                string uri = string.Concat(this.neispuoSettings.BaseUrl, path);

                HttpRequestMessage httpRequest = new()
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(uri)
                };

                HttpClientHandler handler = new()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12
                };

                using var publicKey = new X509Certificate2(this.neispuoSettings.Certificate.CertificatePath);

                string privateKeyText = (await File.ReadAllTextAsync(this.neispuoSettings.Certificate.PrivateKeyPath))
                    .Split("-", StringSplitOptions.RemoveEmptyEntries)[1];

                byte[] privateKeyBytes = Convert.FromBase64String(privateKeyText);

                using var rsa = RSA.Create();

                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

                X509Certificate2 keyPair = publicKey.CopyWithPrivateKey(rsa);
                X509Certificate2 certificate = new(keyPair.Export(X509ContentType.Pfx));

                handler.ClientCertificates.Add(certificate);

                HttpClient httpClient = new(handler)
                {
                    Timeout = TimeSpan.FromSeconds(this.neispuoSettings.CommandTimeout)
                };

                return await httpClient.SendAsync(httpRequest);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}

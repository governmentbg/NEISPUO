namespace SB.ExtApi.IntegrationTests;

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public abstract class ExtApiTestClientBase
{
    protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync([SuppressMessage("", "IDE0060")]CancellationToken ct)
    {
        var request = new HttpRequestMessage();
        request.Headers.TryAddWithoutValidation("X-Client-Cert", RequestCertificates.SchoolBooksProviderCertificateString);
        request.Headers.TryAddWithoutValidation("X-Forwarded-Proto", "HTTPS");
        request.Headers.TryAddWithoutValidation("X-Forwarded-For", "127.0.0.1");
        return Task.FromResult(request);
    }
}

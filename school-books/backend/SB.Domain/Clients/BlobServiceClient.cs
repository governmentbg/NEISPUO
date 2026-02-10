namespace SB.Domain;

using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class BlobServiceClient
{
    // if a string response is larger than 1MB we'd better throw an Exception
    private const long MaxResponseBufferSizeInBytes = 1024 * 1024;

    private static readonly JsonSerializerOptions jsonSerializerOptions = new(JsonSerializerDefaults.Web);
    private static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

    private readonly HttpClient httpClient;
    private readonly string hmacKey;

    public BlobServiceClient(HttpClient httpClient, IOptions<DomainOptions> options)
    {
        this.httpClient = httpClient;
        this.hmacKey = options.Value.BlobServiceHMACKey
            ?? throw new Exception($"Missing setting SB:Domain:BlobServiceHMACKey");
    }

    public record UploadBlobDO(string Name, long Size, int BlobId, string Location);
    public async Task<UploadBlobDO> UploadBlobAsync(
        Stream blobStream,
        string fileName,
        CancellationToken ct)
    {
        using MultipartFormDataContent multipartFormDataContent = new();
        using StreamContent blobContent = new(blobStream);
        multipartFormDataContent.Add(blobContent, "file", fileName);

        string body;
        try
        {
            HttpResponseMessage response = await this.httpClient.PostAsync(
                $"/{CreateUploadQuery(this.hmacKey)}",
                multipartFormDataContent,
                ct);

            // buffer the content so that we can read it as string and
            // throw an exception if its larger than expected
            await response.Content.LoadIntoBufferAsync(MaxResponseBufferSizeInBytes);
            body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"BlobServiceClient GET request non successful status code recieved.\nStatusCode: {response.StatusCode}\nBody: {body}");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception(
                "BlobServiceClient POST request failed.",
                ex);
        }

        return JsonSerializer.Deserialize<UploadBlobDO>(
            body,
            jsonSerializerOptions)
            ?? throw new Exception("BlobServiceClient POST request null json returned as body");
    }

    public record DownloadBlobDO(string Name);
    public async Task<DownloadBlobDO> DownloadBlobToStreamAsync(
        int blobId,
        Stream blobStream,
        CancellationToken ct)
    {
        try
        {
            HttpResponseMessage response = await this.httpClient.GetAsync(
                $"/{blobId}{CreateDownloadQuery(blobId, this.hmacKey)}",
                ct);

            if (!response.IsSuccessStatusCode)
            {
                // buffer the content so that we can read it as string and
                // throw an exception if its larger than expected
                await response.Content.LoadIntoBufferAsync(MaxResponseBufferSizeInBytes);
                string errBody = await response.Content.ReadAsStringAsync(ct);

                throw new Exception($"BlobServiceClient GET request non successful status code recieved.\nStatusCode: {response.StatusCode}\nBody: {errBody}");
            }

            var contentDisposition = response.Content.Headers.ContentDisposition;
            string fileName =
                contentDisposition?.FileNameStar
                ?? contentDisposition?.FileName
                ?? throw new Exception("Missing filename in blob service response.");

            using Stream body = await response.Content.ReadAsStreamAsync(ct);
            await body.CopyToAsync(blobStream, ct);

            return new DownloadBlobDO(fileName);
        }
        catch (HttpRequestException ex)
        {
            throw new Exception(
                "BlobServiceClient GET request failed.",
                ex);
        }
    }

    public async Task DeleteBlobAsync(
        int blobId,
        CancellationToken ct)
    {
        try
        {
            HttpResponseMessage response = await this.httpClient.DeleteAsync(
                $"/{blobId}{CreateDownloadQuery(blobId, this.hmacKey)}",
                ct);

            if (!response.IsSuccessStatusCode)
            {
                // buffer the content so that we can read it as string and
                // throw an exception if its larger than expected
                await response.Content.LoadIntoBufferAsync(MaxResponseBufferSizeInBytes);
                string errBody = await response.Content.ReadAsStringAsync(ct);

                throw new Exception($"BlobServiceClient DELETE request non successful status code recieved.\nStatusCode: {response.StatusCode}\nBody: {errBody}");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception(
                "BlobServiceClient GET request failed.",
                ex);
        }
    }

    private static string CreateUploadQuery(string hmacKey)
    {
        long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return CreateQuery($"{unixTimeSeconds}", unixTimeSeconds, hmacKey);
    }

    private static string CreateDownloadQuery(int blobId, string hmacKey)
    {
        long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return CreateQuery($"{blobId}/{unixTimeSeconds}", unixTimeSeconds, hmacKey);
    }

    private static string CreateQuery(string message, long unixTimeSeconds, string hmacKey)
    {
        byte[] hmac =
            HMACSHA256.HashData(
                USAsciiStrict.GetBytes(hmacKey),
                USAsciiStrict.GetBytes(message));

        string urlSafeBase64HMAC =
            Convert.ToBase64String(hmac)
            // Url-safe Base64 / RFC 4648
            // https://tools.ietf.org/html/rfc4648
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        return $"?t={unixTimeSeconds}&h={urlSafeBase64HMAC}";
    }
}

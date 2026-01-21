namespace SB.Domain;

using System;
using System.Security.Cryptography;
using System.Text;

public static class BlobUtils
{
    public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

    public static string CreateBlobDownloadUrl(string hmacKey, string blobServiceUrl, int blobId)
    {
        long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string message = $"{blobId}/{unixTimeSeconds}";

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

        return $"{blobServiceUrl}/{blobId}?t={unixTimeSeconds}&h={urlSafeBase64HMAC}";
    }
}

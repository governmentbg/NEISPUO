namespace SB.Domain;

using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

public class BlobPublicUrlCreator
{
    private static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

    private readonly string blobServicePublicUrl;
    private readonly string blobServiceHMACKey;

    public BlobPublicUrlCreator(IOptions<DomainOptions> optionsAccessor)
    {
        var options = optionsAccessor.Value;
        this.blobServicePublicUrl =
            options.BlobServicePublicUrl
            ?? throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServicePublicUrl)} is a required option when using {nameof(BlobPublicUrlCreator)}");
        this.blobServiceHMACKey = options.BlobServiceHMACKey
            ?? throw new Exception($"{nameof(DomainOptions)}.{nameof(DomainOptions.BlobServiceHMACKey)} is a required option when using {nameof(BlobPublicUrlCreator)}");
    }

    public string Create(int blobId)
        => Create(this.blobServicePublicUrl, this.blobServiceHMACKey, blobId);

    private static string Create(string blobServiceWebUrl, string blobServiceHMACKey, int blobId)
    {
        long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        string message = $"{blobId}/{unixTimeSeconds}";

        byte[] hmac = HMACSHA256.HashData(
            USAsciiStrict.GetBytes(blobServiceHMACKey),
            USAsciiStrict.GetBytes(message));

        string urlSafeBase64HMAC = ToUrlSafeBase64(hmac);

        return $"{blobServiceWebUrl.TrimEnd('/')}/{blobId}?t={unixTimeSeconds}&h={urlSafeBase64HMAC}";
    }

    private static string ToUrlSafeBase64(byte[] data)
        => Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}

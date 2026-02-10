using Diplomas.Public.Models.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Diplomas.Public.Services.Extensions
{
    public class DocumentExtensions
    {
        public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

        public static string CalcHmac(int blobId, BlobServiceConfig blobServiceConfig)
        {
            if (blobServiceConfig == null) return null;

            string HMACKey = blobServiceConfig.HMACKey; 

            long unixTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            string message = $"{blobId}/{unixTimeSeconds}";

            using HMACSHA256 hash = new HMACSHA256(USAsciiStrict.GetBytes(HMACKey));
            byte[] hmac = hash.ComputeHash(USAsciiStrict.GetBytes(message));
            string urlSafeBase64HMAC =
                Convert.ToBase64String(hmac)
                // Url-safe Base64 / RFC 4648
                // https://tools.ietf.org/html/rfc4648
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');

            var location = $"{blobServiceConfig.Url}/{blobId}?t={unixTimeSeconds}&h={urlSafeBase64HMAC}";
            return location;

        }
    }
}

namespace SB.Blobs;

using System;
using System.Text;
using System.Security.Cryptography;

public static class BlobUtils
{
    public static readonly Encoding USAsciiStrict = Encoding.GetEncoding(
        "us-ascii",
        new EncoderExceptionFallback(),
        new DecoderExceptionFallback());

    public static byte[] GetHMAC(byte[] messageBytes, byte[] keyBytes)
    {
        using HMACSHA256 hash = new HMACSHA256(keyBytes);
        return hash.ComputeHash(messageBytes);
    }

    // Url-safe Base64 / RFC 4648
    // https://tools.ietf.org/html/rfc4648
    public static string ToUrlSafeBase64(byte[] data)
        => Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public static byte[] FromUrlSafeBase64(string dataString)
    {
        string incoming = dataString.Replace('_', '/').Replace('-', '+');

        switch(dataString.Length % 4)
        {
            case 2:
                incoming += "==";
                break;
            case 3:
                incoming += "=";
                break;
        }

        return Convert.FromBase64String(incoming);
    }
}

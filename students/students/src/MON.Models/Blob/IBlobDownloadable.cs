namespace MON.Models
{
    public interface IBlobDownloadable
    {
        public int? BlobId { get; set; }

        public long UnixTimeSeconds { get; set; }

        /// <summary>
        /// // Url-safe Base64 / RFC 4648
        /// https://tools.ietf.org/html/rfc4648
        /// </summary>
        public string Hmac { get; set; }

        public string BlobServiceUrl { get; set; }
    }
}

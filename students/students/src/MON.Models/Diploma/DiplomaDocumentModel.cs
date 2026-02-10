namespace MON.Models.Diploma
{
    public class DiplomaDocumentModel : IBlobDownloadable
    {
        public int? Id { get; set; }
        public int DiplomaId { get; set; }
        public int? BlobId { get; set; }
        public string Description { get; set; }

        public long UnixTimeSeconds { get; set; }

        /// <summary>
        /// // Url-safe Base64 / RFC 4648
        /// https://tools.ietf.org/html/rfc4648
        /// </summary>
        public string Hmac { get; set; }

        public string BlobServiceUrl { get; set; }
    }
}

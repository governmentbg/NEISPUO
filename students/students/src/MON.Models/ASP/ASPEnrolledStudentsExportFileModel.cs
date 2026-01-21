using System;

namespace MON.Models.ASP
{
    public class ASPEnrolledStudentsExportFileModel : IBlobDownloadable
    {
        public int Id { get; set; }
        public short? Month { get; set; }
        public short SchoolYear { get; set; }
        public DateTime CreatedDate { get; set; }
        public int RecordsCount { get; set; }
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

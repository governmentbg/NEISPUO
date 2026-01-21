using System;

namespace MON.Models.HealthInsurance
{
    public class HealthInsuranceExportFileModel : IBlobDownloadable
    {
        public int Id { get; set; }
        public short Year { get; set; }
        public int Month { get; set; }
        public int? BlobId { get; set; }
        public int RecordsCount { get; set; }
      
        public DateTime CreateDate { get; set; }
        public long UnixTimeSeconds { get; set; }
        /// <summary>
        /// // Url-safe Base64 / RFC 4648
        /// https://tools.ietf.org/html/rfc4648
        /// </summary>
        public string Hmac { get; set; }
        public string BlobServiceUrl { get; set; }
        public string MonthName { get; set; }
        public string CreatorUsername { get; set; }
    }
}

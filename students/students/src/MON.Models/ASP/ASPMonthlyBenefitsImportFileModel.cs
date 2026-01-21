namespace MON.Models.ASP
{
    using System;

    public class ASPMonthlyBenefitsImportFileModel : IBlobDownloadable
    {
        public int Id { get; set; }
        public string Month { get; set; }
        public int SchoolYear { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; }
        public string FileStatusCheck { get; set; }
        public int FileStatusCheckId { get; set; }
        public int RecordsCount { get; set; }
        public int ForReview { get; set; }
        public bool ImportCompleted { get; set; }
        public string ImportFileMessages { get; set; }
        public int? BlobId { get; set; }
        public long UnixTimeSeconds { get; set; }

        /// <summary>
        /// // Url-safe Base64 / RFC 4648
        /// https://tools.ietf.org/html/rfc4648
        /// </summary>
        public string Hmac { get; set; }

        public string BlobServiceUrl { get; set; }

        public ASPMonthlyBenefitsImportFileModel ExportedFile { get; set; }
        public bool IsSigned { get; set; }
        public DateTime? SignedDate { get; set; }
        public int AspConfirmSessionCount { get; set; }
        public int MonConfirmSessionCount { get; set; }
        public int? AspSessionNo { get; set; }
        public int? MonSessionNo { get; set; }
    }
}

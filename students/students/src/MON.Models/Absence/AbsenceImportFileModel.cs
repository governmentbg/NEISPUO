namespace MON.Models.Absence
{
    using MON.Shared;
    using MON.Shared.Enums;
    using System;

    public class AbsenceImportFileModel : IBlobDownloadable
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int SchoolYear { get; set; }

        /// <summary>
        /// Дата на последна модификаця (създаване)
        /// </summary>
        public DateTime TimestampUtc { get; set; }
        public int RecordsCount { get; set; }
        public int? BlobId { get; set; }

        public long UnixTimeSeconds { get; set; }

        /// <summary>
        /// // Url-safe Base64 / RFC 4648
        /// https://tools.ietf.org/html/rfc4648
        /// </summary>
        public string Hmac { get; set; }

        public string BlobServiceUrl { get; set; }
        public string SchoolYearName { get; set; }
        public bool IsFinalized { get; set; }
        public bool IsSigned { get; set; }
        public DateTime? FinalizedDate { get; set; }
        public DateTime? SignedDate { get; set; }
        public byte? ImportTypeId { get; set; }
        public bool HasActiveCampaign { get; set; }
        public string ImportType => ImportTypeId.HasValue ? ((AbsenceImportTypeEnum)ImportTypeId.Value).GetEnumDescriptionAttrValue() : string.Empty;

        public DateTime CreateDate { get; set; }
        public string MonthName { get; set; }
    }
}

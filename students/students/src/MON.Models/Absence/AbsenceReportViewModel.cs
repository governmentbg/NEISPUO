namespace MON.Models.Absence
{
    using System;

    public class AbsenceReportViewModel : IBlobDownloadable
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionAbbreviation { get; set; }
        public int? RegionId { get; set; }
        public string InstitutionTown { get; set; }
        public string InstitutionMunicipality { get; set; }
        public string InstitutionRegion { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public short Month { get; set; }
        public string AbsenceCampaignName { get; set; }
        public string AbsenceCampaignDescription { get; set; }
        public DateTime AbsenceCampaignFromDate { get; set; }
        public DateTime AbsenceCampaignToDate { get; set; }
        public bool AbsenceCampaignIsManuallyActivated { get; set; }
        public bool? AbsenceCampaignIsActive { get; set; }
        public int? AbsenceImportId { get; set; }
        public int? AbsenceImportBlobId { get; set; }
        public int AbsenceImportRecordsCount { get; set; }
        public bool AbsenceImportIsSigned { get; set; }
        public bool AbsenceImportIsFinalized { get; set; }
        public DateTime? AbsenceImportSignedDate { get; set; }
        public DateTime? AbsenceImportFinalizedDate { get; set; }
        public DateTime? AbsenceImportCreateDate { get; set; }
        public bool? AbsenceImportSubmitted { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? AbsenceImportCmodifyDate { get; set; }
        public string UpdaterUsername { get; set; }

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

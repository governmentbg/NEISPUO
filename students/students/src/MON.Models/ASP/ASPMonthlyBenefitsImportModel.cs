namespace MON.Models.ASP
{
    using System;

    public class ASPMonthlyBenefitsImportModel : ASPMonthlyBenefitModel
    {
        public int ImportedBlobId { get; set; }
        public int ExportedBlobId { get; set; }
        public DateTime ExportedDate { get; set; }
        public int RecordsCount { get; set; }
    }
}

namespace MON.Models.ASP
{
    using System.Collections.Generic;

    public class ASPMonthlyBenefitsImportResultModel
    {
        public ICollection<ASPMonthlyBenefitsImportModel> Benefits { get; set; }
        public string Errors { get; set; }
    }
}

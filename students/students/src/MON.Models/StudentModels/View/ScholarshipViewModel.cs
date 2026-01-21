namespace MON.Models.StudentModels
{
    using MON.Shared.Extensions;

    public class ScholarshipViewModel : StudentScholarshipModel
    {
        public string SchoolYearName { get; set; }
        public string InstitutionName { get; set; }
        public string ScholarshipTypeName { get; set; }
        public string ScholarshipFinancingOrganName { get; set; }
        public string ScholarshipAmountName { get; set; }
        public bool IsLodFinalized { get; set; }
        public string AmountRateStr => AmountRate.ToExtString(Currency, AltAmountRate, AltCurrency);
    }
}

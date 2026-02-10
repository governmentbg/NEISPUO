namespace MON.Models.ASP
{
    public class ASPMonthlyBenefitReportDetailsDto
    {
        public int CurrentInstitutionId { get; set; }
        public string CurrentInstitutionCode { get; set; }
        public string CurrentInstitutionName { get; set; }
        public string CurrentInstitutionAbbreviation { get; set; }
        public string CurrentInstitutionDetailedSchoolType { get; set; }
        public string CurrentInstitutionTown { get; set; }
        public int CurrentInstitutionRegionId { get; set; }
        public string CurrentInstitutionEmail { get; set; }
        public string CurrentInstitutionPhone { get; set; }
        public int AllRecords { get; set; }
        public int Pending { get; set; }
        public int Confirmed { get; set; }
        public int Rejected { get; set; }
        public bool IsSigned { get; set; }
    }
}

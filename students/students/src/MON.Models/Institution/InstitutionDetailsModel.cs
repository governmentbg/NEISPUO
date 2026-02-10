namespace MON.Models.Institution
{

    using System.Collections.Generic;

    public class InstitutionDetailsModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Bulstat { get; set; }
        public List<ClassGroupBaseModel> Classes { get; set; }
        public string SchoolYear { get; set; }
        public int BaseSchoolTypeId { get; set; }
        public string BaseSchoolType { get; set; }
        public string FinancialSchoolType { get; set; }
        public string DetailedSchoolType { get; set; }
        public string BudgetingSchoolType { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
        public string LocalArea { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public List<PhoneDetails> Phones { get; set; }
        public List<InstitutionDepartmentDetails> Departments { get; set; }
        public List<InstitutionPublicCouncilDetails> PublicCouncil { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public int? RegionId { get; set; }
        public string Region { get; set; }
        public string InstTypeName { get; set; }
        public string InstTypeAbbreviation { get; set; }
        public string BaseSchoolTypeName { get; set; }
        public string DetailedSchoolTypeName { get; set; }
        public string FinancialSchoolTypeName { get; set; }
        public string BudgetingSchoolTypeName { get; set; }
    }
}

namespace MON.Models.ASP
{
    using System;

    public class ASPMonthlyBenefitModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string StudentIdentificationType { get; set; }
        public string Identification { get; set; }
        public string PublicEduNumber { get; set; }
        public int SchoolYear { get; set; }
        public string Month { get; set; }
        public decimal AbsenceCount { get; set; }
        public short AspStatusId { get; set; }
        public DropdownViewModel ASPStatus { get; set; }
        public string Reason { get; set; }
        public short DaysCount { get; set; }
        public short? OnlineEnvironmentDays { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string CurrentInstitutionName { get; set; }
        public short NeispuoStatusId { get; set; }
        public DropdownViewModel NeispuoStatus { get; set; }
        public decimal? AbsenceCorrection { get; set; }
        public short? DaysCorrection { get; set; }
        public decimal AbsencesForTheCurrentMonth { get; set; }
        public int AspMonthlyBenefitsImportId { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public int? CurrentInstitutionId { get; set; }
        public string CurrentClassName { get; set; }
        public int? BasicClassId { get; set; }
        public string BasicClassName { get; set; }
    }
}

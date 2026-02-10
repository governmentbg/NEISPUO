namespace MON.Models.Ores
{
    using System;

    public class OresViewModel : OresModel
    {
        public string OresTypeName { get; set; }
        public string FullName { get; set; }
        public string Pin { get; set; }
        public string PinTypeName { get; set; }
        public bool IsInheritedFromClass { get; set; }
        public bool IsInheritedFromInstitution { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public string InstitutionName { get; set; }
        public string ClassName { get; set; }
        public string BasicClassName { get; set; }
        public string EduFormName { get; set; }
        public DateTime? PersonOresStartDate { get; set; }
        public DateTime? PersonOresEndDate { get; set; }
        public int? PersonOresCalendarDaysCount { get; set; }
        public int? PersonOresWorkDaysCount { get; set; }
        public string TownName { get; set; }
        public string MunicipalityName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public string Uid { get; set; }
        public bool HasManagePermission { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
    }
}

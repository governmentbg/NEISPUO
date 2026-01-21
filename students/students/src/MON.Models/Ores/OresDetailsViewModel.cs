namespace MON.Models.Ores
{
    using System;

    public class OresDetailsViewModel : OresModel
    {
        public string OresTypeName { get; set; }
        public short SchoolYear { get; set; }
        public string InstitutionName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public int StudentsCount { get; set; }
        public string CalendarEventTitle { get; set; }
        public bool? IsInheritedFromInstitution { get; set; }
        public bool? IsInheritedFromClass { get; set; }
        public string StudentFullName { get; set; }
        public string ClassName { get; set; }

        public string GetCalendarEventTitle(int? userInstitutionID)
        {
            return IsInheritedFromInstitution ?? false
                ? InstitutionName
                : (IsInheritedFromClass ?? false
                    ? (userInstitutionID.HasValue ? ClassName : $"{ClassName} - {InstitutionName}")
                    : (userInstitutionID.HasValue ? StudentFullName : $"{StudentFullName} - {InstitutionName}"));
        }
    }
}

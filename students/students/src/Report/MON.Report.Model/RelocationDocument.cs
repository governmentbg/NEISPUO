using System;

namespace MON.Report.Model
{
    public class RelocationDocument
    {
        public string InstitutionName { get; set; }

        public string InstitutionMunicipality { get; set; }

        public string InstitutionCityOrVillage { get; set; }

        public string InstitutionDistrict { get; set; }

        public string InstitutionRegion { get; set; }

        public string OutgoingNumber { get; set; }

        public string OutgoingNumberYear { get; set; }

        public string PersonalId { get; set; }

        public string FullName { get; set; }
        public DateTime? BirthDate { get; set; }

        public string PersonMunicipality { get; set; }

        public string PersonCityOrVillage { get; set; }

        public string PersonRegion { get; set; }

        public string PreparationGroup { get; set; }

        public string PrepGroupFrom { get; set; }

        public DateTime? PrepGroupTo { get; set; }

        public string SchoolYear { get; set; }

        public string AcademicYearTo { get; set; }
        public string PersonalIdType { get; set; }
        public string BirthPlaceTown { get; set; }
        public string BirthPlaceMinicipality { get; set; }
        public string BirthPlaceRegion { get; set; }
        public string EduOrganization { get; set; }
    }
}

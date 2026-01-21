namespace MON.Models.StudentModels
{
    using System;

    public class AdmissionPermissionRequestViewModel : AdmissionPermissionRequestModel
    {
        public string RequestingInstitutionName { get; set; }
        public string AuthorizingInstitutionName { get; set; }
        public string PersonName { get; set; }
        public DateTime CreateDate { get; set; }
        public string RequestingInstitutionAbbreviation { get; set; }
        public string AuthorizingInstitutionAbbreviation { get; set; }
        public string RequestingInstitutionTown { get; set; }
        public string RequestingInstitutionMunicipality { get; set; }
        public string RequestingInstitutionRegion { get; set; }
    }
}

namespace MON.Models.DocManagement
{
    public class DocManagementRequestReportModel : DocManagementApplicationReportModel
    {
        public string today { get; set; }
        public string institutionRepresentative { get; set; }
        public string requestedInstitutionName { get; set; }
        public string requestedInstitutionTown { get; set; }
        public string requestedInstitutionMunicipality { get; set; }
        public string requestedInstitutionRegion { get; set; }
        public string requestedInstitutionRepresentative { get; set; }
    }
}

namespace MON.Models
{
    public class AdmissionDocumentViewModel : AdmissionDocumentModel
    {
        public RelocationDocumentViewModel RelocationDocument { get; set; }
        public string InstitutionName { get; set; }
        public string StatusName { get; set; }
        public string AdmissionReasonTypeName { get; set; }
        public bool UsedInClassEnrollment { get; set; }
        public bool IsReferencedInStudentClass { get; set; }
        public string PositionName { get; set; }
        public string SchoolYearName { get; set; }
    }
}

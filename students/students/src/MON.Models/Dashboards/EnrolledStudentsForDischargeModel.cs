using System;

namespace MON.Models.Dashboards
{
    public class EnrolledStudentsForDischargeModel
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public string PinType { get; set; }
        public string Pin { get; set; }
        public int? AdmissionDocumentId { get; set; }
        public int PersonId { get; set; }
        public int? NewInstitutonId { get; set; }
        public string NewInstituton { get; set; }
        public string NewPosition { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public DateTime NoteDate { get; set; }
        public int? OldInstitutionId { get; set; }
        public string OldInstitution { get; set; }
        public int? RelocationDocumentId { get; set; }
        public int? DischargeDocumentId { get; set; }
        public int? RegionId { get; set; }
    }
}

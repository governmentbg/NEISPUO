using System;

namespace MON.Models.Dashboards
{
    public class StudentForAdmissionModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public int? AdmissionDocumentId { get; set; }
        public string FullName { get; set; }
        public int? InstitutionId { get; set; }
        public string PinType { get; set; }
        public string Pin { get; set; }
        public string Gender { get; set; }
        public string PositionName { get; set; }
        public DateTime? NoteDate { get; set; }
        public int? PositionId { get; set; }
        public bool? CanBeEnrolled { get; set; }
        public string InstitutionName { get; set; }
        public string RegionName { get; set; }
        public int? PersonAge { get; set; }
        public DateTime? PersonBirthDate { get; set; }
        public string MainClassName { get; set; }
        public int? MainClassId { get; set; }
    }
}

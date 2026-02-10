using System;

namespace MON.Models.StudentModels.Class
{
    public class ClassStudentsToEnrollDto
    {
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public string Pin { get; set; }
        public string PinType { get; set; }
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int PositionId { get; set; }
        public string PositionName { get; set; }
        public int? AdmissionDocumentId { get; set; }
        public DateTime? AdmissionDocumentNoteDate { get; set; }
        public int? Age { get; set; }
        public DateTime? BirthDate { get; set; }
        public string MainClassName { get; set; }
        public int? MainClassId { get; set; }
    }
}

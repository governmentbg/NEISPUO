using System;
using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class StudentGeneralTrainingDataDetails
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string Institution { get; set; }
        public int PositionId { get; set; }
        public string Position { get; set; }
        public int? AdmissionDocumentId { get; set; }
        public int? AdmissionRelocationDocumentId { get; set; }
        public short SchoolYear { get; set; }
        public List<int> DischargeDocumentsIds { get; set; }
        public List<StudentMovementDocumentBasicDetails> RelocationDocuments { get; set; }
    }

    public class StudentMovementDocumentBasicDetails
    {
        public int Id { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public DateTime? NoteDate { get; set; }
        public string NoteNumber { get; set; }
        public string ReasonType { get; set; }
        public string SendingInstitution { get; set; }
        public string HostInstitution { get; set; }
        public string RuoOrderNumber { get; set; }
        public DateTime? RuoOrderDate { get; set; }

        /// <summary>
        /// Тип на документ (AdmissionDocument, DischargeDocument, RelocationDocument)
        /// </summary>
        public string DocumentType { get; set; }
        public StudentMovementDocumentBasicDetails RelatedDocument { get; set; }
    }
}

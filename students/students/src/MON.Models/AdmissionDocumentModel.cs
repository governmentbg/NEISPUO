using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MON.Models
{
    public class AdmissionDocumentModel : IInstitutionNotNullable
    {
        public int? Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        [MaxLength(20)]
        public string NoteNumber { get; set; }

        [Required]
        public DateTime NoteDate { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        public int Status { get; set; }

        public int? RelocationDocumentId { get; set; }

        [Required]
        public int InstitutionId { get; set; }

        public int AdmissionReasonTypeId { get; set; }

        public IEnumerable<DocumentModel> Documents { get; set; }
        public int Position { get; set; }
        public string ClassEnrolledIn { get; set; }
        public bool CanBeModified { get; set; }
        public bool CanBeEnrolled { get; set; }
        public int CreatedBySysUserId { get; set; }
        public bool HasHealthStatusDocument { get; set; }
        public bool HasImmunizationStatusDocument { get; set; }
    }
}

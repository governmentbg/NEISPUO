using MON.Models.Interfaces;
using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MON.Models
{
    public class DischargeDocumentModel : IInstitution, IDocumentModels
    {
        public int? Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        [MaxLength(20)]
        public string NoteNumber { get; set; }

        [Required]
        public DateTime NoteDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        public int DischargeReasonTypeId { get; set; }

        public IEnumerable<DocumentModel> Documents { get; set; }

        public int? InstitutionId { get; set; }

        public int? CurrentStudentClass { get; set; }

        public string CurrentStudentClassName { get; set; }

        public int? StudentClassId { get; set; }

        public int Status { get; set; }

        public bool IsDraft => Status == (int)DocumentStatus.Draft;

        public string InstitutionName { get; set; }
        public string SchoolYearName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MON.Models
{
    public class RelocationDocumentModel
    {
        public int? Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        [MaxLength(20)]
        public string NoteNumber { get; set; }

        [Required]
        public DateTime NoteDate { get; set; }

        public int Status { get; set; }

        /// <summary>
        /// Id на приемаща институция
        /// </summary>
        public int? InstitutionId { get; set; }
        /// <summary>
        /// Име на приемаща институция
        /// </summary>
        public string InstitutionName { get; set; }

        public int? CurrentStudentClassId { get; set; }
        public string CurrentStudentClassName { get; set; }

        public int? RelocationReasonTypeId { get; set; }

        public int? SendingInstitutionId { get; set; }
        public string SendingInstitution { get; set; }

        public IEnumerable<DocumentModel> Documents { get; set; }

        public IEnumerable<AdmissionDocumentGeneralModel> AdmissionDocumentModels { get; set; }

        public string RUOOrderNumber { get; set; }

        public DateTime? RUOOrderDate { get; set; }

        public DateTime? DischargeDate { get; set; }

        public bool CanBeModified { get; set; }

    }
}

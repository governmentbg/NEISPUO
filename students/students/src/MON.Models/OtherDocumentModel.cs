using MON.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MON.Models
{
    public class OtherDocumentModel : IInstitution
    {
        public int? Id { get; set; }
        public int ResourceSupportId { get; set; }
        public string Description { get; set; }
        public string Series { get; set; }
        public string FactoryNumber { get; set; }
        public string RegNumberTotal { get; set; }
        public string RegNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        [Required]
        public int PersonId { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int? DocumentTypeId { get; set; }
        public string DocumentTypeName { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
        public short? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public bool IsLodFinalized { get; set; }
    }
}

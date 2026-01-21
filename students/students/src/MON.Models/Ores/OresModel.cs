namespace MON.Models
{
    using System;
    using System.Collections.Generic;

    public class OresModel
    {
        public int? Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int OresTypeId { get; set; }
        public string Description { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }
        public int? PersonId { get; set; }
        public int? ClassId { get; set; }
        public int? InstitutionId { get; set; }
    }
}

namespace MON.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class BasicDocumentSequenceModel
    {
        public int BasicDocumentId { get; set; }
        public int? InstitutionId { get; set; }
        public DateTime RegDate { get; set; }
        public int RegNumberTotal { get; set; }
        public int RegNumberYear { get; set; }
    }

    public class BasicDocumentSequenceViewModel: BasicDocumentSequenceModel
    {
        public int Id { get; set; }
        public string InstitutionName { get; set; }
        public int? RegionId { get; set; }
        public short SchoolYear { get; set; }
        public string BasicDocumentName { get; set; }
        public int? DiplomaId { get; set; }
        public int? PersonId { get; set; }
        public string FullName { get; set; }
        public bool IsUsed { get; set; }
    }
}

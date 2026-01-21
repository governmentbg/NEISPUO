namespace MON.Models.StudentModels
{
    using System;
    using System.Collections.Generic;

    public class InternationalMobilityModel
    {
        public int? Id { get; set; }
        public string Project { get; set; }
        public string ReceivingInstitution { get; set; }
        public string MainObjectives { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PersonId { get; set; }
        public int CountryId { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }
        public int? InstitutionId { get; set; }
        public short SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
    }
}

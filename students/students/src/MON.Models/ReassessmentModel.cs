namespace MON.Models
{
    using System;
    using System.Collections.Generic;

    public class ReassessmentModel
    {
        public int Id { get; set; }

        public int PersonId { get; set; }

        public int ReasonId { get; set; }
        public string ReasonName { get; set; }

        public int? InClass { get; set; }

        public short? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }

        public string Reason { get; set; }

        public string BasicClassName { get; set; }

        public DateTime CreationDate { get; set; }
        public IEnumerable<ReassessmentDetailsModel> ReassessmentDetails { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }

    }
}

using System;
using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class StudentResourceSupportModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public string ReportNumber { get; set; }
        public DateTime ReportDate { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }
        public List<ResourceSupportModel> ResourceSupports { get; set; } = new List<ResourceSupportModel>();
    }
}

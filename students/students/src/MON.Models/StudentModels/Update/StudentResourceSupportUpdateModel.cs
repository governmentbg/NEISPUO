namespace MON.Models.StudentModels.Update
{
    using MON.Models.StudentModels.ResourceSupport;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class StudentResourceSupportUpdateModel
    {
        [Required]
        public int PersonId { get; set; }
        public short? SchoolYear { get; set; }
        public List<ResourceSupportReportModel> ResourceSupportReports { get; set; }
    }
}

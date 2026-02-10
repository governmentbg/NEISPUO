namespace MON.Models.StudentModels.ResourceSupport
{
    using System;
    using System.Collections.Generic;

    public class ResourceSupportReportModel
    {
        public int Id { get; set; }
        public string ReportNumber { get; set; }
        public DateTime ReportDate { get; set; }
        public int SchoolYear { get; set; }
        public int PersonId { get; set; }
        public int? SysUserID { get; set; }
        public List<ResourceSupportModel> ResourceSupportDetails { get; set; }
        public List<ResourceSupportDocumentModel> ResourceSupportDocuments { get; set; }
    }
}

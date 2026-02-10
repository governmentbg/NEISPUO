using MON.Models.StudentModels.ResourceSupport;
using System.Collections.Generic;

namespace MON.Models.StudentModels
{

    public class ResourceSupportViewModel
    {
        public List<ResourceSupportReportModel> ResourceSupportReports { get; set; }
        public bool HasResourceSupport => ResourceSupportReports != null && ResourceSupportReports.Count > 0;
        public List<ResourceSupportReportModel> ResourceSupportReportsPreviousYears { get; set; }

    }
}

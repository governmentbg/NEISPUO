using System;

namespace MON.Models.DocManagement
{
    public class DocManagementCampaignModel
    {
        public int? Id { get; set; }
        public int? ParentId { get; set; }
        public int? InstitutionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short SchoolYear { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsManuallyActivated { get; set; }
    }
}

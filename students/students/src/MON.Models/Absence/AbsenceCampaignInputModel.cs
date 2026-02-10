using MON.Shared;
using System;

namespace MON.Models.Absence
{
    public class AbsenceCampaignInputModel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short SchoolYear { get; set; }
        public short Month { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsManuallyActivated { get; set; }
    }
}

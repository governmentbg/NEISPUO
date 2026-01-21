using System.Collections.Generic;

namespace MON.Models.Dashboards
{
    public class ClassGroupStatsModel
    {
        public int Total { get; set; }
        public Dictionary<string, int> ClassGroupsByType { get; set; }
    }
}

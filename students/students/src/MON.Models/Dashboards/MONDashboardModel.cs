using System.Collections.Generic;

namespace MON.Models.Dashboards
{
    public class MONDashboardModel
    {
        public StudentStatsModel StudentsData { get; set; }
        public List<ClassGroupStatsModel> StudentClassesData { get; set; }
    }
}

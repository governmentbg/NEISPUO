using System.Collections.Generic;

namespace MON.Models.Dashboards
{
    public class RUODashboardModel
    {
        public StudentStatsModel StudentsData { get; set; }
        public List<ClassGroupStatsModel> StudentClassesData { get; set; }
    }
}

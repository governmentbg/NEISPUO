using System.Collections.Generic;

namespace MON.Models.Dashboards
{
    public class StudentStatsModel
    {
        public Dictionary<string, int> StudentsByPosition { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public int Total { get; set; }
        public List<StudentStatsModel> Children { get; set; }
        public int ClassTypeId { get; set; }
        public int? PositionId { get; set; }
    }
}

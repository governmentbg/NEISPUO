namespace MON.Models
{
    using System.Collections.Generic;

    public class StudentClassesTimelineInputModel
    {
        public int PersonId { get; set; }
        public HashSet<int?> Positions { get; set; } = new HashSet<int?>();
        public int? InstitutionId { get; set; }
        public short? SchoolYear { get; set; }
        public int? ClassKind { get; set; }
    }
}

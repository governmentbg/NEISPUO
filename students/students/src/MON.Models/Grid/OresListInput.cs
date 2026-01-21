namespace MON.Models.Grid
{
    using System;

    public class OresListInput : PagedListInput
    {
        public int? PersonId { get; set; }

        public int? ClassId { get; set; }

        public int? InstitutionId { get; set; }

        public short? SchoolYear { get; set; }

        public int? OresTypeId { get; set; }

        public int? OresId { get; set; }
        public string InheritanceType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}

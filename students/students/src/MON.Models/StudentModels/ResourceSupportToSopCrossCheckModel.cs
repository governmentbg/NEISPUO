namespace MON.Models.StudentModels
{
    public class ResourceSupportToSopCrossCheckModel
    {
        public int PersonId { get; set; }
        public int SchoolYear { get; set; }
        public int? SpecialNeedsYearId { get; set; }
        public int? ResourceSupportReportId { get; set; }
    }
}

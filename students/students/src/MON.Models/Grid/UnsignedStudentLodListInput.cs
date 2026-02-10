namespace MON.Models.Grid
{
    public class UnsignedStudentLodListInput : PagedListInput
    {
        public short SchoolYear { get; set; }
        public int? InstitutionId { get; set; }
        public int? RegionId { get; set; }
    }
}

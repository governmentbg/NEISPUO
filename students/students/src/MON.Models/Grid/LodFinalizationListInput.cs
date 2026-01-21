namespace MON.Models.Grid
{

    public class LodFinalizationListInput : PagedListInput
    {
        public LodFinalizationListInput()
        {
            SortBy = "SchoolYear desc";
        }

        public int? PersonId { get; set; }
        public int? InstitutionId { get; set; }

        public short? SchoolYear { get; set; }
    }
}

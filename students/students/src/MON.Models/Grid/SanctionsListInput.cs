namespace MON.Models.Grid
{
    public class SanctionsListInput : PagedListInput
    {
        public SanctionsListInput()
        {
            SortBy = "StartDate desc";
        }

        public int? PersonId { get; set; }
    }
}

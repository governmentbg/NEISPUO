namespace MON.Models.Grid
{
    public class RecognitionsListInput : PagedListInput
    {
        public RecognitionsListInput()
        {
            SortBy = "Id desc";
        }

        public int PersonId { get; set; }
    }
}

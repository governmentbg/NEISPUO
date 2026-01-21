namespace MON.Models.Grid
{

    public class NotesListInput : PagedListInput
    {
        public NotesListInput()
        {
            SortBy = "IssueDate desc";
        }

        public int PersonId { get; set; }
    }
}

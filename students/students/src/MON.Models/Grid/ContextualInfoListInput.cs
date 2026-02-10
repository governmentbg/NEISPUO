namespace MON.Models.Grid
{
    public class ContextualInfoListInput : PagedListInput
    {
        public ContextualInfoListInput()
        {
            SortBy = "Key asc";
        }

        public string ModuleName { get; set; }
    }
}

namespace MON.Models.Grid
{
    public class LogsListInput : PagedListInput
    {
        public LogsListInput()
        {
            SortBy = "TimestampUtc desc";
        }
    }
}

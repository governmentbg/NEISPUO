namespace Helpdesk.Models.Grid
{
    public class PagedListInput : IPagedListInput
    {
        public PagedListInput()
        {
            PageIndex = 0;
            PageSize = 10;
        }

        public string Filter { get; set; }

        public string SortBy { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}

namespace Helpdesk.Models.Grid
{
    public interface IPagedListInput
    {
        string Filter { get; set; }

        string SortBy { get; set; }

        int PageIndex { get; set; }

        int PageSize { get; set; }
    }
}

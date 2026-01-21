namespace MON.Models
{
    public class MessagesListInput : PagedListInput
    {
        public MessagesListInput()
        {
            SortBy = "IsRead desc, Id desc";
        }
    }
}

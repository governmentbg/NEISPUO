namespace MON.Models.Grid
{
    public class RefugeeApplicationListInput : PagedListInput
    {
        public RefugeeApplicationListInput()
        {
            SortBy = "ApplicationDate desc, Status asc";
        }
    }
}

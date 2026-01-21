namespace MON.Models.Grid
{
    using MON.Models.Finance;
    using System.Collections.Generic;

    public class ResourceSupportDataDataListInput : PagedListInput
    {
        public ResourceSupportDataDataListInput()
        {
            SortBy = "";
        }

        public List<string> Periods { get; set; }
    }
}

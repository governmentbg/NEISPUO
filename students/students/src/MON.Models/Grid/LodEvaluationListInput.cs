namespace MON.Models.Grid
{
    using System.Collections.Generic;

    public class LodEvaluationListInput : PagedListInput
    {
        public int PersonId { get; set; }
        public List<int> SchoolYears { get; set; }
    }
}

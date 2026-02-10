namespace MON.Models.Grid
{
    public class HealthInsuranceDataListInput : PagedListInput
    {
        public HealthInsuranceDataListInput()
        {
            SortBy = "";
        }

        public short? Year { get; set; }

        public short? Month { get; set; }
    }
}

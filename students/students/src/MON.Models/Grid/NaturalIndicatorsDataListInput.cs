namespace MON.Models.Grid
{
    using MON.Models.Finance;
    using System.Collections.Generic;

    public class NaturalIndicatorsDataListInput : PagedListInput
    {
        public NaturalIndicatorsDataListInput()
        {
            SortBy = "";
        }

        public List<string> Periods { get; set; }
        public bool ShowItemPrice { get; set; }
        public bool ShowItemValue { get; set; }
        public short? Year { get; set; }

        public short? Period { get; set; }
    }

}

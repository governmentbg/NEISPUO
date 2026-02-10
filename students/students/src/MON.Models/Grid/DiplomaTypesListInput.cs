namespace MON.Models.Grid
{
    public class DiplomaTypesListInput : PagedListInput
    {
        public DiplomaTypesListInput()
        {
            SortBy = "Name asc";
        }

        public bool? HasSchema { get; set; }
        public bool? HasBarcode { get; set; }
        public bool? IsValidation { get; set; }
        public bool? IsAppendix { get; set; }
        public bool? IsDuplicate { get; set; }
        public bool? IsIncludedInRegister { get; set; }
        public bool? IsRuoDoc { get; set; }
    }
}

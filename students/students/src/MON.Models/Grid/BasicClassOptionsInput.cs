namespace MON.Models.Grid
{
    public class BasicClassOptionsInput
    {
        public string SearchStr { get; set; }
        public int? SelectedValue { get; set; }
        public int? MinId { get; set; }
        public int? MaxId { get; set; }
        public int? PageSize { get; set; }
    }
}

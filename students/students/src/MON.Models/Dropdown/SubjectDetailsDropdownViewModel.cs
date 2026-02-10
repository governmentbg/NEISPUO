namespace MON.Models.Dropdown
{
    public class SubjectDetailsDropdownViewModel : DropdownViewModel
    {
        public int SubjectId { get; set; }
        public int? SubjectTypeId { get; set; }
        public int? PartId { get; set; }
    }
}

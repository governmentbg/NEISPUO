namespace MON.Models.Dropdown
{
    public class GradeDropdownViewModel : DropdownViewModel
    {
        public bool IsSpecialGrade { get; set; }
        public int? GradeTypeId { get; set; }
        public string GradeTypeName { get; set; }
    }
}

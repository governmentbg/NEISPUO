namespace MON.Models.Institution
{
    public class InstitutionDropdownViewModel : DropdownViewModel
    {
        public int BaseSchoolTypeId { get; set; }
        public int DetailedSchoolTypeId { get; set; }
        public string Town { get; set; }
        public string Municipality { get; set; }
        public string Region { get; set; }
        public int? RegionId { get; set; }
        public string LocalArea { get; set; }
        public string Details { get; set; }
        public string DetailedSchoolTypeName { get; set; }
    }
}

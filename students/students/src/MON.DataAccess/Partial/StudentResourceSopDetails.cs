namespace MON.DataAccess
{
    public class StudentResourceSopDetails
    {
        public int PersonId { get; set; }
        public string SpecialNeedsType { get; set; }
        public string SpecialNeedsSubType { get; set; }
        public string SupportiveEnvironment { get; set; }
        public bool? HasSuportiveEnvironment { get; set; }
        public int? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
    }
}

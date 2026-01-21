namespace MON.DataAccess
{
    public partial class TmpStudents
    {
        public double StudentId { get; set; }
        public int? ClassId { get; set; }
        public int? StudentSpecialityId { get; set; }
        public string StudentSpecialityName { get; set; }
        public int? StudentEduFormId { get; set; }
        public string StudentEduFormName { get; set; }
        public int? SchoolId { get; set; }
        public int? ClassIdOrig { get; set; }
        public int? GroupIdOrig { get; set; }
    }
}

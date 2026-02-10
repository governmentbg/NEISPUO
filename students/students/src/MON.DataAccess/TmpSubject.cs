// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MON.DataAccess
{
    public partial class TmpSubject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameAbreviation { get; set; }
        public int? SchoolId { get; set; }
        public short? SubjectIdOrig { get; set; }
    }
}

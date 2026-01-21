using System.Collections.Generic;

namespace MON.DataAccess
{
    public partial class TmpSubjects
    {
        public TmpSubjects()
        {
            StudentEvaluation = new HashSet<StudentEvaluation>();
        }

        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectNameAbreviation { get; set; }
        public int? SchoolId { get; set; }
        public short? SubjectIdOrig { get; set; }

        public virtual ICollection<StudentEvaluation> StudentEvaluation { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
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

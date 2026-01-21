using System.Collections.Generic;

namespace MON.DataAccess
{
    public partial class Class
    {
        public Class()
        {
            DiplomEvaluation = new HashSet<DiplomEvaluation>();
        }

        public int Id { get; set; }
        public int Position { get; set; }
        public int ClassTypeId { get; set; }
        public int TemplateId { get; set; }
        public int? SysUserId { get; set; }

        public virtual ClassType ClassType { get; set; }
        public virtual SysUser SysUser { get; set; }
        public virtual Template Template { get; set; }
        public virtual ICollection<DiplomEvaluation> DiplomEvaluation { get; set; }
    }
}

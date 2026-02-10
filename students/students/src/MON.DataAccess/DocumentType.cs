using System;
using System.Collections.Generic;

namespace MON.DataAccess
{
    public partial class DocumentType
    {
        public DocumentType()
        {
            Template = new HashSet<Template>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsValid { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public virtual ICollection<Template> Template { get; set; }
    }
}

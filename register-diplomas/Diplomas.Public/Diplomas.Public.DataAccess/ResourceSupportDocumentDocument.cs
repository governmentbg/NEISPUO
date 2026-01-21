using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class ResourceSupportDocumentDocument
    {
        public int Id { get; set; }
        public int ResourceSupportDocumentId { get; set; }
        public int DocumentId { get; set; }

        public virtual Document Document { get; set; }
        public virtual ResourceSupportDocument ResourceSupportDocument { get; set; }
    }
}

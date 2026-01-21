// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MON.DataAccess
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

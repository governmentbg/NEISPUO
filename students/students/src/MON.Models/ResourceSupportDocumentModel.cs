namespace MON.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ResourceSupportDocumentModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        [Required]
        public int ResourceSupportReportId { get; set; }
        public DocumentViewModel Document { get; set; }
    }
}

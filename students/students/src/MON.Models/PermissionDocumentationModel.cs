using System.ComponentModel.DataAnnotations;

namespace MON.Models
{
    public class PermissionDocumentationModel
    {
        [Required(AllowEmptyStrings = false)]
        public string PermissionName { get; set; }
        public string Permission { get; set; }
        public string Description { get; set; }
        public string Usage { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MON.Models.Diploma
{
    public class DiplomaTemplateCreateModel
    {
        public int BasicDocumentTypeId { get; set; }
        public int InstitutionId { get; set; }
        public string Contents { get; set; }
        [Required]
        public string Name { get; set; }
        public string SubjectContents { get; set; }
    }
}

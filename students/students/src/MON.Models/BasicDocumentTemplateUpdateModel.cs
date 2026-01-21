using MON.Models.Diploma;
using MON.Models.Dynamic;
using System.Collections.Generic;

namespace MON.Models
{
    public class BasicDocumentTemplateUpdateModel
    {
        public int? Id { get; set; }
        public string BasicDocumentName { get; set; }
        public List<BasicDocumentPartUpdateModel> BasicDocumentParts { get; set; }
        public List<DynamicEntitySection> Schema { get; set; }
        public List<DiplomaTemplateSubjectPartModel> SubjectSections { get; set; }
    }
}

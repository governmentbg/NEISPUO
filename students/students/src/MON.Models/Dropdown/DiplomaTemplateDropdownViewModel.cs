using MON.Models.Diploma;
using MON.Shared;
using MON.Shared.Extensions;
using System.Collections.Generic;

namespace MON.Models
{
    public class DiplomaTemplateDropdownViewModel : DropdownViewModel
    {
        public string BasicDocumentContentsJsonStr { get; set; }
        public string TemplateContentsJsonStr { get; set; }
        public string TemplateSubjectContentsJsonStr { get; set; }
        public IEnumerable<DiplomaTemplateSubjectPartModel> DocumentTemplatePartsWithSubjects { get; set; }
        public IEnumerable<BasicDocumentPartModel> DocumentParts { get; set; }
        public int? BasicClassId { get; set; }
        public string BasicClassName { get; set; }
        public string BasicClassesStr { get; set; }

        public HashSet<int> BasicClassList => BasicClassesStr.IsNullOrEmpty()
            ? new HashSet<int>()
            : BasicClassesStr.Split('|', System.StringSplitOptions.RemoveEmptyEntries).ToHashSet<int>();
    }
}

namespace MON.Models.Diploma
{
    using MON.Shared;
    using System.Collections.Generic;
    using System.Linq;

    public class BasicDocumentTemplatePartModel : BasicDocumentPartUpdateModel
    {
        public bool HasSubjectTypeLimit => SubjectTypes != null && SubjectTypes.Count > 0;
        public bool HasExternalEvaluationLimit => ExternalEvaluationTypes != null && ExternalEvaluationTypes.Count > 0;
        public List<DropdownViewModel> SubjectTypeOptions { get; set; }
        public List<BasicDocumentTemplateSubjectModel> Subjects { get; set; }
        public HashSet<int> ExcludedPositions => Subjects.IsNullOrEmpty() 
            ? null 
            : Subjects.Where(x => !x.SubjectCanChange).Select(x => x.Position).ToHashSet();

        public bool ShowEctsGrade { get; set; }
    }
}

using MON.Models.Dropdown;

namespace MON.Models.StudentModels
{
    public class LodEvaluationSectionBaseModel
    {
        public int? Id { get; set; }
        public int OrderNum { get; set; }
        public SubjectDetailsDropdownViewModel Subject { get; set; }
    }
}

namespace MON.Models.StudentModels
{
    using MON.Models.Dropdown;
    using System.Collections.Generic;

    public class LodEvaluationsViewModel
    {
        public IEnumerable<LodEvaluationSectionABModel> LodEvaluationSectionABModels { get; set; }
        public IEnumerable<LodEvaluationSectionVModel> LodEvaluationSectionVModels { get; set; }
        public IEnumerable<LodEvaluationSectionGModel> LodEvaluationSectionGModels { get; set; }
        public IEnumerable<KeyValuePair<string, List<ProfileSubjectDetailsDropdownViewModel>>> LodEvaluationProfileSubjectModels { get; set; }
        public LodEvaluationGeneralModel LodEvaluationGeneralModel { get; set; }
    }
}

namespace MON.Models.StudentModels
{
    using MON.Models.Dropdown;
    using System.Collections.Generic;

    public class LodSelfEduFormEvaluationsViewModel
    {
        public IEnumerable<LodSelfEduFormEvaluationModel> LodSelfEduFormEvaluationModels { get; set; }
        public IEnumerable<LodEvaluationSectionGModel> LodEvaluationSectionGModels { get; set; }
        public LodEvaluationGeneralModel LodEvaluationGeneralModel { get; set; }
        public IEnumerable<KeyValuePair<string, List<ProfileSubjectDetailsDropdownViewModel>>> LodEvaluationProfileSubjectModels { get; set; }

    }
}

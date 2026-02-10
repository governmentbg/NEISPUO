using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class PersonalDevelopmentSupportModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public string EarlyEvaluationAndEducationalRiskInfo { get; set; }
        public string AdditionalModulesNeededForNonBulgarianSpeakingInfo { get; set; }
        public string EvaluationConclusionInfo { get; set; }
        public int? SupportPeriodTypeId { get; set; }
        public int? StudentTypeId { get; set; }
        public IEnumerable<PersonalDevelopmenReasonsModel> EarlyEvaluationReasons { get; set; }
        public IEnumerable<PersonalDevelopmenReasonsModel> CommonSupportTypeReasons { get; set; }
        public IEnumerable<PersonalDevelopmenReasonsModel> AdditionalSupportTypeReasons { get; set; }
        public IEnumerable<DocumentModel> AdditionalSupportDocuments { get; set; }
        public IEnumerable<DocumentModel> CommonSupportDocuments { get; set; }
        public IEnumerable<DocumentModel> EarlyEvaluationDocuments { get; set; }
    }
}

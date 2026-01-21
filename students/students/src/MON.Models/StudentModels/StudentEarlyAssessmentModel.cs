namespace MON.Models.StudentModels
{
    using System.Collections.Generic;

    public class StudentEarlyAssessmentModel
    {
        public int PersonId { get; set; }
        public string AdditionalInfo { get; set; }
        public string BgAdditionalTrainingInfo { get; set; }
        public string ConclusionInfo { get; set; }
        public StudentEarlyAssessmentLearningDisabilityModel LearningDisability { get; set; }
        public List<StudentEarlyAssessmentDisabilityReasonModel> DisabilityReasons { get; set; }
        public IEnumerable<DocumentViewModel> Documents { get; set; }
    }
}

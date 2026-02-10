namespace Domain.Models
{
    using System.Collections.Generic;

    public class PreSchoolReadinessResultsSingleYearModel
    {
        public PreSchoolReadinessResultsSingleYearModel()
        {
            subjects = new List<PreSchoolReadinessResultsModel>();
        }

        public int basicClassId { get; set; }
        public string basicClassDescription { get; set; }
        public string schoolYear { get; set; }

        public List<PreSchoolReadinessResultsModel> subjects { get; set; }
    }

    public class PreSchoolReadinessResultsModel
    {
        public string endOfYearEvaluation { get; set; }
        public string subjectName { get; set; }
    }
}

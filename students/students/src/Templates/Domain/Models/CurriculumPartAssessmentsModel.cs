namespace Domain.Models
{
    using System.Collections.Generic;

    public class CurriculumPartAssessmentsModel
    {
        public CurriculumPartAssessmentsModel()
        {
            subjectsAssessments = new List<SubjectAssessmentsModel>();
        }

        public string curriculumPart { get; set; }

        public List<SubjectAssessmentsModel> subjectsAssessments { get; set; }
    }
}

namespace Domain.Models
{
    using System.Collections.Generic;

    public class SchoolYearAssessmentsModel
    {
        public SchoolYearAssessmentsModel()
        {
            curriculumParts = new List<CurriculumPartAssessmentsModel>();
        }

        public string schoolYear { get; set; }
        public string basicClass { get; set; }

        public List<CurriculumPartAssessmentsModel> curriculumParts { get; set; }
    }
}

namespace Domain.Models
{
    using System.Collections.Generic;

    public class SchoolYearExternalEvaluationsModel
    {
        public SchoolYearExternalEvaluationsModel()
        {
            externalEvaluationItems = new List<ExternalEvaluationItemSubjectModel>();
        }

        public string schoolYear { get; set; }
        public string name { get; set; }

        public List<ExternalEvaluationItemSubjectModel> externalEvaluationItems { get; set; }
    }
}

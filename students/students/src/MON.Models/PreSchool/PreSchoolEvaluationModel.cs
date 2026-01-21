namespace MON.Models.PreSchool
{
    public class PreSchoolEvaluationModel
    {
        public int? Id { get; set; }
        public int BasicClassId { get; set; }
        public int PersonId { get; set; }
        public int? SubjectId { get; set; }
        public short SchoolYear { get; set; }

        public string StartOfYearEvaluation { get; set; }
        public string EndOfYearEvaluation { get; set; }
    }

    public class PreSchoolEvaluationViewModel : PreSchoolEvaluationModel
    {
        public string BasicClass { get; set; }
        public string Subject { get; set; }
        public string SchoolYearName { get; set; }
        public bool EnteredStartOfYearEvaluation { get; set; }
        public bool EnteredEndOfYearEvaluation { get; set; }
    }
}

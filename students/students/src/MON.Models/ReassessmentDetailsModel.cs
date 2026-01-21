namespace MON.Models
{
    public class ReassessmentDetailsModel
    {
        public int? Id { get; set; }
        public int SortOrder { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string GradeName { get; set; }
        public int? SubjectTypeId { get; set; }
        public string SubjectTypeName { get; set; }
        public string Uid { get; set; }
        public int GradeCategory { get; set; }
        public decimal? Grade { get; set; }
        public int? SpecialNeedsGrade { get; set; }
        public int? OtherGrade { get; set; }
        public decimal? QualitativeGrade { get; set; }

    }
}

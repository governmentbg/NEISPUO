namespace MON.Report.Model
{
    using System.Net.Cache;

    public class SubjectEvaluationPrintModel
    {
        public int SubjectId { get; set; }
        public int? CurriculumPartId { get; set; }
        public int? BasicClassId { get; set; }
        public string SubjectName { get; set; }
        public int? SubjectTypeId { get; set; }
        public string SubjectTypeName { get; set; }
        public string CurriculumPartName { get; set; }
        public int SortOrder { get; set; }

        public SubjectEvaluationGradeModel FirstTermEvaluationGrade { get; set; }
        public SubjectEvaluationGradeModel SecondTermEvaluationGrade { get; set; }
        public SubjectEvaluationGradeModel AnnualEvaluationGrade { get; set; }
        public SubjectEvaluationGradeModel FifthGradeEvaluationGrade { get; set; }
        public SubjectEvaluationGradeModel SixthGradeEvaluationGrade { get; set; }


        public string FirstTermEvaluationStr => FirstTermEvaluationGrade?.GradeText ?? "";
        public string SecondTermEvaluationStr => SecondTermEvaluationGrade?.GradeText ?? "";
        public string AnnualEvaluationStr => AnnualEvaluationGrade?.GradeText ?? "";
        public string FifthGradeAnnualEvaluationStr => FifthGradeEvaluationGrade?.GradeText ?? "";
        public string SixthGradeAnnualEvaluationStr => SixthGradeEvaluationGrade?.GradeText ?? "";
    }

    public class SubjectEvaluationGradeModel
    {
        public int? GradeId { get; set; }
        public string GradeText { get; set; }
    }
}
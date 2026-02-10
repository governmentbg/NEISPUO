namespace MON.Models.StudentModels
{
    public class LodEvaluationSectionGModel : LodEvaluationSectionBaseModel
    {
        public int? SectionGFirstTermGrade { get; set; }
        public int? SectionGSecondTermGrade { get; set; }
        public int? SectionGFinalGrade { get; set; }
        public int? SectionGHours { get; set; }
        public bool IsSelfEduForm { get; set; }
    }
}
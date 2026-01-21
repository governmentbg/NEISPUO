namespace MON.Models.StudentModels
{
    public class LodSelfEduFormEvaluationModel : LodEvaluationSectionBaseModel
    {
        public int? SectionASession0Grade { get; set; }
        public int? SectionASession1Grade { get; set; }
        public int? SectionASession2Grade { get; set; }
        public int? SectionASession3Grade { get; set; }
        public int? SectionAFinalGrade { get; set; }
        public int? SectionBSession0Grade { get; set; }
        public int? SectionBSession1Grade { get; set; }
        public int? SectionBSession2Grade { get; set; }
        public int? SectionBSession3Grade { get; set; }
        public int? SectionBFinalGrade { get; set; }
        public int? SectionAHours { get; set; }
        public int? SectionBHours { get; set; }
    }
}
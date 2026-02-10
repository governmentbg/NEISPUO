namespace MON.Models.StudentModels.Lod
{

    public class LodAssessmentImportListModel : LodAssessmentModel
    {
        public string FullName { get; set; }
        public string Pin { get; set; }
        public string PinTypeName { get; set; }
        public string SubjectName { get; set; }
        public string SubjectTypeName { get; set; }
        public string CurriculumPartName { get; set; }
        public string SchoolYearName { get; set; }
        public string GradeCategoryName { get; set; }
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public string Uid { get; set; }
        public bool IsModule { get; set; }
        public decimal? DecimalGrade { get; set; }
        public bool IsProfSubject { get; set; }
    }
}

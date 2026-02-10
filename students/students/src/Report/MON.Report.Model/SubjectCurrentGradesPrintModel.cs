namespace MON.Report.Model
{

    public class SubjectCurrentGradesPrintModel
    {
        public string SubjectName { get; set; }
        public string FirstTermGrades { get; set; }
        public string SecondTermGrades { get; set; }
        public int? CurriculumPartId { get; set; }
        public string CurriculumPartName { get; set; }
        public int SortOrder { get; set; }
    }
}

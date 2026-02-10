namespace MON.Models.StudentModels
{
    public class StudentLodEvaluationListDto
    {
        public int? StudentClassId { get; set; }
        public short? SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public string ClassName { get; set; }
        public string SpecialityName { get; set; }
        public string ProfessionName { get; set; }
        public string EduFormName { get; set; }
        public string BasicClassName { get; set; }
        public string BasciClassRomeName { get; set; }
        public string BasciClassDescription { get; set; }
        public string ClassTypeName { get; set; }
        public string EvaluationTypes { get; set; }
        public string SourceTypes { get; set; }
    }
}

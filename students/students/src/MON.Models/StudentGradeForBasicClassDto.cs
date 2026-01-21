namespace MON.Models
{
    public class StudentGradeForBasicClassDto
    {
        public int PersonId { get; set; }
        public int InstitutionId { get; set; }
        public int? BasicClassId { get; set; }
        public short SchoolYear { get; set; }
        public int? CurriculumPartId { get; set; }
        public string CurriculumPart { get; set; }
        public string CurriculumPartName { get; set; }
        public int CurriculumId { get; set; }
        public int SubjectId { get; set; }
        public int? SubjectTypeId { get; set; }
        public string SubjectName { get; set; }
        public string BasicSubjectName { get; set; }
        public string BasicSubjectAbrev { get; set; }
        public string FirstTermDecimalGrades { get; set; }
        public string SecondTermDecimalGrades { get; set; }
        public string FirstTermQualitativeGrades { get; set; }
        public string SecondTermQualitativeGrades { get; set; }
        public string FirstTermSpecialGrades { get; set; }
        public string SecondTermSpecialGrades { get; set; }
    }
}

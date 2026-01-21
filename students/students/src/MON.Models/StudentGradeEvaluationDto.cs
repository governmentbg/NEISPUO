namespace MON.Models
{
    public class StudentGradeEvaluationDto
    {
        public int? PersonId { get; set; }
        public int? CurriculumPartId { get; set; }
        public string CurriculumPart { get; set; }
        public string CurriculumPartName { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string BasicSubjectName { get; set; }
        public string BasicSubjectAbrev { get; set; }
        public int? BasicClassId { get; set; }
        public bool IsValidForBasicClass { get; set; }
        public int? InstitutionId { get; set; }
        public short? SchoolYear { get; set; }
        public decimal? GradeFirstTerm { get; set; }
        public decimal? GradeSecondTerm { get; set; }
        public decimal? FinalGrade { get; set; }
    }
}

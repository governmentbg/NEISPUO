namespace MON.Models.Absence
{
    public class StudentClassAbsencesDto
    {
        public int StudentClassId { get; set; }
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public int ClassId { get; set; }
        public bool StudentClassIsCurrent { get; set; }
        public string ClassName { get; set; }
        public int? ClassNumber { get; set; }
        public short SchoolYear { get; set; }
        public int InstitutionId { get; set; }
        public int? AbsenceId { get; set; }
        public short? Month { get; set; }
        public short? Excused { get; set; }
        public short? Unexcused { get; set; }
        public bool LodIsFinalized { get; set; }
    }
}

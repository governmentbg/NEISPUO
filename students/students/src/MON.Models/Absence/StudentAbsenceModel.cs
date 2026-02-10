namespace MON.Models.Absence
{
    public class StudentAbsenceModel : StudentAbsenceInputModel
    {
        public string Name { get; set; }
        public int? ClassNumber { get; set; }
        public int InstitutionId { get; set; }
        public bool StudentClassIsCurrent { get; set; }
        public bool IsLodFinalized { get; set; }
    }
}

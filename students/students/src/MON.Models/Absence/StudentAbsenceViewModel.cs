namespace MON.Models.Absence
{
    public class StudentAbsenceViewModel : StudentAbsenceInputModel
    {
        public string SchoolYearName { get; set; }
        public string MonthName { get; set; }
        public bool IsLodFinalized { get; set; }
    }
}
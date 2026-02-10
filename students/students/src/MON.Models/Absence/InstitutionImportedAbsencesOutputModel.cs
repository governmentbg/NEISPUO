namespace MON.Models.Absence
{
    public class InstitutionImportedAbsencesOutputModel
    {
        public string Name { get; set; }

        public int? AbsenceId { get; set; }

        public string InstitutionName { get; set; }

        public short? SchoolYear { get; set; }

        public string SchoolYearName { get; set; }
        public string MonthName { get; set; }
        public short? Month { get; set; }
    }
}
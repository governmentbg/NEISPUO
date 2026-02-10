namespace MON.Models.Absence
{
    public class StudentAbsenceInputModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public int ClassId { get; set; }
        public short SchoolYear { get; set; }
        public short Month { get; set; }
        public decimal Excused { get; set; }
        public decimal Unexcused { get; set; }
    }
}

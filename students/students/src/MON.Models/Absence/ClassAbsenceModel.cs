namespace MON.Models.Absence
{
    using System.Collections.Generic;

    public class ClassAbsenceModel
    {
        public ClassAbsenceModel()
        {
            StudentAbsences = new List<StudentAbsenceModel>();
        }

        public int ClassId { get; set; }
        public short SchoolYear { get; set; }
        public short Month { get; set; }
        public string Name { get; set; }
        public bool IsValid { get; set; }
        public List<StudentAbsenceModel> StudentAbsences { get; set; }
    }
}

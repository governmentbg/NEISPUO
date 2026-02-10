namespace MON.DataAccess.Dto
{
    using System;

    public class StudentClassTemporal
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public bool IsCurrent { get; set; }
        public int ClassId { get; set; }
        public int InstitutionId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? DischargeDate { get; set; }
        public int BasicClassId { get; set; }
        public int StudentEduFormId { get; set; }
    }
}

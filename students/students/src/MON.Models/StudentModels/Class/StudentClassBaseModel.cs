namespace MON.Models
{
    using System;

    public class StudentClassBaseModel
    {
        public int? Id { get; set; }
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public int ClassId { get; set; }
        public int? SelectedClassTypeId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? DischargeDate { get; set; }
    }
}

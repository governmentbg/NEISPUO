using System;

namespace MON.Models.Absence
{
    public class StudentAbsenceHistoryOutputModel
    {
        public decimal Excused { get; set; }
        public decimal Unexcused { get; set; }
        public DateTime CreateDate { get; set; }
        public string Username { get; set; }
    }
}

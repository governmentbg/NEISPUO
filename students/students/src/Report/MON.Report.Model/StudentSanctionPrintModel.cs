namespace MON.Report.Model
{
    using System;

    public class StudentSanctionPrintModel
    {
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string StartDateStr => StartDate.ToString("dd.MM.yyyy");
        public string EndDateStr => EndDate.HasValue ? EndDate.Value.ToString("dd.MM.yyyy") : "-";
    }
}

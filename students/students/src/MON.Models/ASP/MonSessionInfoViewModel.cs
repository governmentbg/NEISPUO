namespace MON.Models.ASP
{
    using System;

    public class MonSessionInfoViewModel
    {
        public int SessionNo { get; set; }
        public DateTime TargetMonth { get; set; }
        public string InfoType { get; set; }
        public DateTime? AspProcessed { get; set; }
        public int ConfirmationRecordsCount { get; set; }

        public int Year => TargetMonth.Year;
        public int Month => TargetMonth.Month;

        public DateTime? MonProcessed { get; set; }
    }
}

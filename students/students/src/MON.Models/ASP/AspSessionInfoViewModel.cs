namespace MON.Models.ASP
{
    using System;

    public class AspSessionInfoViewModel
    {
        public int SessionNo { get; set; }
        public DateTime TargetMonth { get; set; }
        public string InfoType { get; set; }
        public DateTime? MonProcessed { get; set; }
        public int AbsenceCount { get; set; }
        public int ZpCount { get; set; }
        public int ConfirmationRecordsCount { get; set; }
        public bool HasConfirmationCampaign { get; set; }

        public int Year => TargetMonth.Year;
        public int Month => TargetMonth.Month;
    }
}

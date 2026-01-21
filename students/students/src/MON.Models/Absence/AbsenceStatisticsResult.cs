namespace MON.Models.Absence
{
    public class AbsenceStatisticsResult
    {
        public int Month { get; set; }
        public int UnsubmittedCount { get; set; }
        public int SubmittedCount { get; set; }
        public int UnsignedCount { get; set; }
        public int SignedCount { get; set; }
        public int All => UnsubmittedCount + SubmittedCount + UnsignedCount + SignedCount;
    }
}

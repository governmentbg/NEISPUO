namespace MON.Models.ASP
{
    public class AspStatisticsResult
    {
        public int Month { get; set; }
        public int Comfirmed { get; set; }
        public int Rejected { get; set; }
        public int ForReview { get; set; }
        public int All => Comfirmed + Rejected + ForReview;
    }
}

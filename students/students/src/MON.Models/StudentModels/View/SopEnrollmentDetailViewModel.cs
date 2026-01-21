namespace MON.Models.StudentModels
{
    public class SopEnrollmentDetailViewModel
    {
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public int? Age { get; set; }
        public string MainInstName { get; set; }
        public int MainInstRegionId { get; set; }
        public string MainInstRegionName { get; set; }
        public int MainInstCode { get; set; }
        public string CsopInstName { get; set; }
        public int? CsopRegionId { get; set; }
        public string CsopRegionName { get; set; }
        public int? CsopCode { get; set; }
        public string Uid { get; set; }
    }
}

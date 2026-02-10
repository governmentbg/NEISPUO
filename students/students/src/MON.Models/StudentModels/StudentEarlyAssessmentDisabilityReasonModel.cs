namespace MON.Models.StudentModels
{
    public class StudentEarlyAssessmentDisabilityReasonModel
    {
        public int? Id { get; set; }
        public int ReasonId { get; set; }
        public string Details { get; set; }
        public string ReasonName { get; set; }
    }
}

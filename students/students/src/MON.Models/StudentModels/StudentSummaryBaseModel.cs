namespace MON.Models.StudentModels
{
    public class StudentSummaryBaseModel
    {
        public int PersonId { get; set; }
        public string FullName { get; set; }
        public bool IsLodApproved { get; set; }
        public bool IsLodFinalized { get; set; }
    }
}

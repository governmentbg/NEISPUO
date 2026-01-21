namespace Helpdesk.Models.Grid
{
    public class IssuePageListInput : PagedListInput
    {
        public int? Category { get; set; }
        public int? Status { get; set; }
        public int? Priority { get; set; }
        public int? AssignLevel { get; set; }
        public bool? IsEscalated { get; set; }
        public int? SupportLevel { get; set; }
        public bool? AssignedToMe { get; set; }
        public bool? SearchEverywhere { get; set; }
        public bool? RequestForInformation { get; set; }
    }
}

namespace Helpdesk.Models.Issue
{
    using System;

    public class IssueStatusHistoryViewModel
    {
        public string Comment { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatorUsername { get; set; }
        public string Uid => Guid.NewGuid().ToString();
    }
}

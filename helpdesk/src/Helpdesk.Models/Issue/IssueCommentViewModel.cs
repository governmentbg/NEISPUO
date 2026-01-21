using System;

namespace Helpdesk.Models.Issue
{
    public class IssueCommentViewModel : IssueCommentModel
    {
        public DateTime CreateDate { get; set; }
        public string CreatorUsername { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string ModifierUsername { get; set; }
        public bool IsResolvingComment { get; set; }
        public string Uid => Guid.NewGuid().ToString();
    }
}

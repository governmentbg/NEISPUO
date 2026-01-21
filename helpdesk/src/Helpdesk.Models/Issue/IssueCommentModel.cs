using System.Collections.Generic;

namespace Helpdesk.Models.Issue
{
    public class IssueCommentModel
    {
        public int IssueId { get; set; }
        public string Comment { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
    }
}

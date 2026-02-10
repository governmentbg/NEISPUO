using System;
using System.Collections.Generic;

namespace Helpdesk.Models.Issue
{
    public class IssueModel
    {
        public int? Id { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? SubcategoryId { get; set; }
        public bool IsEscalated { get; set; }
        public bool IsLevel3Support { get; set; }
        public int? AssignedToSysUserId { get; set; }
        public string AssignedToSysUser { get; set; }
        public DateTime? ResolveDate { get; set; }
        public string Phone { get; set; }
        public string Survey { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
        public bool RequestForInformation { get; set; }
    }
}

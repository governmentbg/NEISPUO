using System;
using System.Collections.Generic;

namespace Helpdesk.Models.Issue
{
    public class IssueViewModel: IssueModel
    {
        public string Category { get; set; }
        public string HtmlDescription { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Subcategory { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string SubmitterUsername { get; set; }
        public int SubmitterSysUserId { get; set; }
        public IEnumerable<IssueCommentViewModel> Comments { get; set; }
        public IEnumerable<IssueStatusHistoryViewModel> StatusHistory { get; set; }
        public int CommentsCount { get; set; }
        public int AttachmentsCount { get; set; }
        public InstitutionModel Institution { get; set; }
        public bool HasUnreadChanges { get; set; }
    }
}

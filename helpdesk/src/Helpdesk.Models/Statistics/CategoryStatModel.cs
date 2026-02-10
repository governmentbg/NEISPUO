namespace Helpdesk.Models.Statistics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public class CategoryStatModel
    {
        public int Id { get; set; }
        public int? Code { get; set; }
        public string Name { get; set; }
        public int? AllIssues { get; set; }
        public int? NewIssues { get; set; }
        public int? AssignedIssues { get; set; }
        public int? ResolvedIssues { get; set; }
        public DateTime? Oldest { get; set; }
    }
}

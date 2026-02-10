using System;
using System.Collections.Generic;

namespace MON.Models.DocManagement
{
    public class DocManagementApplicationStatusModel
    {
        public int? Id { get; set; }
        public int ApplicationId { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int? ParentId { get; set; }
    }
}

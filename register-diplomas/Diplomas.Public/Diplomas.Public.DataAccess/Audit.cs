using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class Audit
    {
        public int AuditId { get; set; }
        public string AuditCorrelationId { get; set; }
        public int AuditModuleId { get; set; }
        public int? SysUserId { get; set; }
        public int? SysRoleId { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string LoginSessionId { get; set; }
        public string RemoteIpAddress { get; set; }
        public string UserAgent { get; set; }
        public DateTime DateUtc { get; set; }
        public int? SchoolYear { get; set; }
        public int? InstId { get; set; }
        public int? PersonId { get; set; }
        public string ObjectName { get; set; }
        public int? ObjectId { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }

        public virtual AuditModule AuditModule { get; set; }
    }
}

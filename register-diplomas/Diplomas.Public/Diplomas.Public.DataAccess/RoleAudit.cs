using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class RoleAudit
    {
        public int AuditId { get; set; }
        public int? SysUserId { get; set; }
        public string Username { get; set; }
        public int? SysRoleId { get; set; }
        public DateTime DateUtc { get; set; }
        public string Action { get; set; }
        public int? InstId { get; set; }
        public string ObjectName { get; set; }
        public int? ObjectId { get; set; }
        public int AuditModuleId { get; set; }
        public string AssignedToSysUserId { get; set; }
        public string AssignedToSysUsername { get; set; }
        public string AssignedSysRoleId { get; set; }
        public string AssignedSysRoleName { get; set; }
        public string AssignedInstitutionId { get; set; }
    }
}

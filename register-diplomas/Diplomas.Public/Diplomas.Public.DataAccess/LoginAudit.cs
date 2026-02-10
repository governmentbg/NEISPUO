using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class LoginAudit
    {
        public int LoginAuditId { get; set; }
        public int SysUserId { get; set; }
        public string Username { get; set; }
        public int SysRoleId { get; set; }
        public string SysRoleName { get; set; }
        public int? InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public int? RegionId { get; set; }
        public string RegionName { get; set; }
        public int? MunicipalityId { get; set; }
        public string MunicipalityName { get; set; }
        public int? BudgetingInstitutionId { get; set; }
        public string BudgetingInstitutionName { get; set; }
        public int? PositionId { get; set; }
        public string Ipsource { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

using System.Collections.Generic;

namespace Helpdesk.Models.Identity
{
    // Данни за текущо логнат потребител
    public class UserInfo
    {
        public string Username { get; set; }
        public int SysUserID { get; set; }
        public int SysRoleID { get; set; }
        public int? InstitutionID { get; set; }
        public int? PositionID { get; set; }
        public int? MunicipalityID { get; set; }
        public int? RegionID { get; set; }
        public int? BudgetingInstitutionID { get; set; }
        public bool? IsLeadTeacher { get; set; }
        public HashSet<int> LeadTeacherClasses { get; set; }
    }
}

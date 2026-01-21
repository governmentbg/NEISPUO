namespace Helpdesk.Services.Implementations
{
    using Helpdesk.Shared.Enums;
    using Helpdesk.Shared.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class UserInfo : IUserInfo
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
        public string ClientIp { get; set; }
        public string UserAgent { get; set; }
        public string AccessToken { get; set; }

        public bool IsInRole(UserRoleEnum role)
        {
            return this.SysRoleID == (int)role;
        }

        public bool IsSchoolDirector => IsInRole(UserRoleEnum.School);
        public bool IsMon => IsInRole(UserRoleEnum.Mon);
        public bool IsMonExpert => IsInRole(UserRoleEnum.MonExpert);
        public bool IsConsortium => IsInRole(UserRoleEnum.Consortium);
        public bool IsRuo => IsInRole(UserRoleEnum.Ruo);
        public bool IsRuoExpert => IsInRole(UserRoleEnum.RuoExpert);
        public bool IsTeacher => IsInRole(UserRoleEnum.Teacher);
        public bool IsInstitutionAssociate => IsInRole(UserRoleEnum.InstitutionAssociate);
        public bool IsCIOO => IsInRole(UserRoleEnum.CIOO);
        public bool IsMunicipality => IsInRole(UserRoleEnum.Municipality);
        public UserRoleEnum UserRole => (UserRoleEnum)this.SysRoleID;
    }
}

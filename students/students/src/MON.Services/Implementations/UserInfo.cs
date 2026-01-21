using MON.Shared.Enums;
using MON.Shared.Interfaces;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MON.Services.Implementations
{
    [ExcludeFromCodeCoverage]
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
        public int PersonId { get; set; }
        public string AccessToken { get; set; }
        public string LoginSessionId { get; set; }

        public bool IsInRole(UserRoleEnum role)
        {
            return SysRoleID == (int)role;
        }

        public bool IsSchoolDirector => IsInRole(UserRoleEnum.School);
        public bool IsMon => IsInRole(UserRoleEnum.Mon) || IsCIOO;
        public bool IsMonExpert => IsInRole(UserRoleEnum.MonExpert);
        public bool IsRuo => IsInRole(UserRoleEnum.Ruo);
        public bool IsRuoExpert => IsInRole(UserRoleEnum.RuoExpert);
        public bool IsTeacher => IsInRole(UserRoleEnum.Teacher);
        public bool IsInstitutionAssociate => IsInRole(UserRoleEnum.InstitutionAssociate);
        public bool IsCIOO => IsInRole(UserRoleEnum.CIOO);
        public bool IsMonOBGUM => IsInRole(UserRoleEnum.MonOBGUM);
        public bool IsMonOBGUM_Finance => IsInRole(UserRoleEnum.MonOBGUM_Finance);
        public bool IsMonHR => IsInRole(UserRoleEnum.MonHR);
        public bool IsConsortium => IsInRole(UserRoleEnum.Consortium);
        public UserRoleEnum UserRole => (UserRoleEnum)SysRoleID;
        public string Impersonator { get; set; }
        public int? ImpersonatorSysUserID { get; set; }
    }
}

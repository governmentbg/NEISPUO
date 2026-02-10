using MON.Shared.Enums;
using System.Collections.Generic;

namespace MON.Shared.Interfaces
{
    public interface IUserInfo
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

        /// <summary>
        /// Ролята е Институция (директор)
        /// </summary>
        public bool IsSchoolDirector { get; }

        /// <summary>
        /// Ролята е Институция (техн. сътрудник)
        /// </summary>
        public bool IsInstitutionAssociate { get; }

        /// <summary>
        /// Ролята е Учител
        /// </summary>
        public bool IsTeacher { get; }

        /// <summary>
        /// Ролята е МОН
        /// </summary>
        public bool IsMon { get; }
        public bool IsMonExpert { get; }
        /// <summary>
        /// Ролята е РУО
        /// </summary>
        public bool IsRuo { get; }
        public bool IsRuoExpert { get; }

        public bool IsCIOO { get; }

        public bool IsMonHR { get; }

        public bool IsInRole(UserRoleEnum role);
        public bool IsConsortium { get; }
        public UserRoleEnum UserRole { get; }
        public string LoginSessionId { get; }

        public string Impersonator { get; set; }
        public int? ImpersonatorSysUserID { get; set; }
    }
}

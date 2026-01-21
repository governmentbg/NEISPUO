using System.Collections.Generic;

namespace MON.Models.Identity
{
    public class UserInfoViewModel : UserInfo
    {
        public string Institution { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string Municipality { get; set; }
        public string Person { get; set; }
        public string Budget { get; set; }
        public string RoleName { get; set; }
        public int? BaseSchoolTypeId { get; set; }
        public string InstType { get; set; }
        public int InstTypeId { get; set; }
        public string SchoolYearName { get; set; }
        public List<ClassGroupInfoModel> LeadTeacherClassGroups { get; set; }
    }

    public class ClassGroupInfoModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

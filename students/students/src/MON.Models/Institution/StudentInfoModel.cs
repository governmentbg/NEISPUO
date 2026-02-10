using MON.Models.StudentModels;

namespace MON.Models.Institution
{
    public class StudentInfoModel : StudentSummaryBaseModel
    {
        public string BirthPlace { get; set; }
        public string Phone { get; set; }
        public bool? HasSpecialNeeds { get; set; }
        public string Usernames { get; set; }
        public string InitialPasswords { get; set; }
        public int ClassId { get; set; }
        public int? ClassNumber { get; set; }
        public string ClassName { get; set; }
        public string EduFormName { get; set; }
        public string MainClassName { get; set; }
    }
}

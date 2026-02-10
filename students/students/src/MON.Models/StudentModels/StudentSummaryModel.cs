using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class StudentSummaryModel : StudentSummaryBaseModel
    {
        public string Gender { get; set; }
        public string PIN { get; set; }
        public string PINType { get; set; }
        public int PintTypeId { get; set; }
        public int Age { get; set; }
        public bool AzureAccountDeleted { get; set; }
        public StudentClassSummaryModel CurrentClass { get; set; }
        public List<StudentClassSummaryModel> AllCurrentClasses { get; set; }
        public List<WaitingToBeDischarged> WaitingToBeDischarged { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
    }
}

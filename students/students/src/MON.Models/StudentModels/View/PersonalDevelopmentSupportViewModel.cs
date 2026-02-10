using System.Collections.Generic;

namespace MON.Models.StudentModels
{
    public class PersonalDevelopmentSupportViewModel
    {
        public int Id { get; set; }
        public int SchoolYear { get; set; }
        public string SchoolYearName { get; set; }
        public IEnumerable<PdsReasonViewModel> EarlyEvaluationReasons { get; set; }
        public IEnumerable<PdsReasonViewModel> CommonSupportTypeReasons { get; set; }
        public IEnumerable<PdsReasonViewModel> AdditionalSupportTypeReasons { get; set; }
        public IEnumerable<DocumentModel> Documents { get; set; }
        public bool IsLodFinalized { get; set; }
    }

    public class PdsReasonViewModel
    {
        public string ReasonName { get; set; }
    }
}

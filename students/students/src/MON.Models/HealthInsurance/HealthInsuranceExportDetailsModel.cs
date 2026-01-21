using System.Collections.Generic;

namespace MON.Models.HealthInsurance
{
    public class HealthInsuranceExportDetailsModel : HealthInsuranceExportFileModel
    {
        public IEnumerable<HealthInsuranceStudentsViewModel> Records { get; set; }
    }
}

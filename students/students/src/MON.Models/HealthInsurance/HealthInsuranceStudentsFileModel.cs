using System.Collections.Generic;

namespace MON.Models.HealthInsurance
{
    public class HealthInsuranceStudentsFileModel
    {
        public short Year { get; set; }
        public short Month { get; set; }
        public IEnumerable<HealthInsuranceStudentsViewModel> HealthInsuranceStudentsModel { get; set; }
    }
}

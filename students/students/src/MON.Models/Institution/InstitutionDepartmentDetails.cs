using System.Collections.Generic;

namespace MON.Models.Institution
{
    public class InstitutionDepartmentDetails
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Town { get; set; }
        public string District { get; set; }
        public string Municipality { get; set; }
        public string Address { get; set; }
        public List<PhoneDetails> Phones { get; set; }
    }
}

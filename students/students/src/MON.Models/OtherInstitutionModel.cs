using System;

namespace MON.Models
{
    public class OtherInstitutionViewModel
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Reason { get; set; }
        public DropdownViewModel Institution { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}

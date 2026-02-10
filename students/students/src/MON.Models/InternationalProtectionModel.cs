using System;

namespace MON.Models
{
    public class InternationalProtectionModel
    {
        public int? Id { get; set; }
        public int ProtectionStatus { get; set; }

        public string DocNumber { get; set; }

        public DateTime? DocDate { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}

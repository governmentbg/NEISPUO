using System;

namespace MON.Models
{

    public class PhoneDetails
    {
        public string Type { get; set; }
        public string ContactKind { get; set; }
        public string Code { get; set; }
        public string Number { get; set; }
        public string Uid => Guid.NewGuid().ToString();
    }
}

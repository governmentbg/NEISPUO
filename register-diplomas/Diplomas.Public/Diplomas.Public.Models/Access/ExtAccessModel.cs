using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models.Access
{
    public class ExtAccessModel
    {
        public int? PersonalIdType { get; set; }
        public string PersonalId { get; set; }
        public string Params { get; set; }
        public string IPAddress { get; set; }
        public bool HasResult { get; set; }
        public int? ExtSystemId { get; set; }
        public int? ExtSystemServiceId { get; set; }
    }
}

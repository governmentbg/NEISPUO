using System;
using System.Collections.Generic;
using System.Text;

namespace Diplomas.Public.Models
{
    public class ErrorModel
    {
        public int? Id { get; set; }
        public short Severity { get; set; }
        public int ModuleId { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string AdditionalInformation { get; set; }
        public int? CreatedBySysUserID { get; set; }
        public DateTime CreateDate { get; set; }
    }
}

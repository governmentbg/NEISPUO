using System;
using System.Collections.Generic;

#nullable disable

namespace Diplomas.Public.DataAccess
{
    public partial class Error
    {
        public int Id { get; set; }
        public short Severity { get; set; }
        public string Module { get; set; }
        public string Message { get; set; }
        public string Trace { get; set; }
        public string Ipaddress { get; set; }
        public string UserAgent { get; set; }
        public string AdditionalInformation { get; set; }
        public int? CreatedBySysUserId { get; set; }
        public DateTime CreateDate { get; set; }

        public virtual SysUser CreatedBySysUser { get; set; }
    }
}

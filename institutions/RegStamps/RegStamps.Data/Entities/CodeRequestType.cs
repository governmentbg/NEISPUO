using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class CodeRequestType
{
    public int RequestTypeId { get; set; }

    public string RequestTypeName { get; set; } = null!;

    public virtual ICollection<TbRequest> TbRequests { get; set; } = new List<TbRequest>();
}

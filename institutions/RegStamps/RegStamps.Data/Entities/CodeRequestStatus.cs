using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class CodeRequestStatus
{
    public int RequestStatusId { get; set; }

    public string RequestStatusName { get; set; } = null!;

    public virtual ICollection<TbRequest> TbRequests { get; set; } = new List<TbRequest>();
}

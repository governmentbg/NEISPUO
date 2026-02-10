using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class CodeStampStatus
{
    public int StampStatusId { get; set; }

    public string StampStatusName { get; set; } = null!;

    public virtual ICollection<TbStamp> TbStamps { get; set; } = new List<TbStamp>();
}

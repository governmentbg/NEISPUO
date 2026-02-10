using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class CodeStampType
{
    public int StampTypeId { get; set; }

    public string StampTypeName { get; set; } = null!;

    public virtual ICollection<TbStamp> TbStamps { get; set; } = new List<TbStamp>();
}

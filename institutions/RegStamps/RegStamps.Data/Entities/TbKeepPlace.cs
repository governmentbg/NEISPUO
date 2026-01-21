using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbKeepPlace
{
    public int PlaceId { get; set; }

    public string? KeepPlaceName { get; set; }

    public virtual ICollection<RefRequestStamp> RefRequestStamps { get; set; } = new List<RefRequestStamp>();
}

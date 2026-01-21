using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbKeeper
{
    public int KeeperId { get; set; }

    public int Idtype { get; set; }

    public string? IdNumber { get; set; }

    public string? Name1 { get; set; }

    public string? Name2 { get; set; }

    public string? Name3 { get; set; }

    public int OccupationId { get; set; }

    public virtual ICollection<RefRequestStamp> RefRequestStamps { get; set; } = new List<RefRequestStamp>();
}

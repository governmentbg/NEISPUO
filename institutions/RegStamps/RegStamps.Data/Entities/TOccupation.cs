using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TOccupation
{
    public int OccupId { get; set; }

    public int NoccupId1 { get; set; }

    public int NoccupId2 { get; set; }

    public int OccupType { get; set; }

    public string OccupName { get; set; } = null!;

    public int CodeOccMon { get; set; }
}

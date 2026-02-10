using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbMoncert
{
    public int Id { get; set; }

    public string PersonName { get; set; } = null!;

    public string CertId { get; set; } = null!;

    public virtual ICollection<RefRequestStamp> RefRequestStamps { get; set; } = new List<RefRequestStamp>();

    public virtual ICollection<TbLog> TbLogs { get; set; } = new List<TbLog>();
}

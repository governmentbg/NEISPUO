using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbLog
{
    public int LogId { get; set; }

    public int? MonCertId { get; set; }

    public string? SignerName { get; set; }

    public string? SignerEmail { get; set; }

    public string? Organisation { get; set; }

    public string? SignerBulstat { get; set; }

    public string? SignerIdent { get; set; }

    public string? Signature { get; set; }

    public DateTime? TimeStamp { get; set; }

    public virtual TbMoncert? MonCert { get; set; }
}

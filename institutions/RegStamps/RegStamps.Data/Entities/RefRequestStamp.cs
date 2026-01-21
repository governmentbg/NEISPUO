using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class RefRequestStamp
{
    public int RequestId { get; set; }

    public int StampId { get; set; }

    public int SchoolId { get; set; }

    public int KeeperId { get; set; }

    public int KeepPlaceId { get; set; }

    public string? OrderNumber { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public string? MonNotes { get; set; }

    public int? MonCertId { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual TbKeepPlace KeepPlace { get; set; } = null!;

    public virtual TbKeeper Keeper { get; set; } = null!;

    public virtual TbMoncert? MonCert { get; set; }

    public virtual TbRequest TbRequest { get; set; } = null!;

    public virtual TbStamp TbStamp { get; set; } = null!;
}

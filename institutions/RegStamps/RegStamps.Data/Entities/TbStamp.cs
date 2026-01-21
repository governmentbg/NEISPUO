using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbStamp
{
    public int StampId { get; set; }

    public int SchoolId { get; set; }

    public int StampType { get; set; }

    public int StampStatus { get; set; }

    public DateTime? FirstUseDate { get; set; }

    public string? FirstUsePerson { get; set; }

    public string? LetterNumber { get; set; }

    public DateTime? LetterDate { get; set; }

    public string? Image { get; set; }

    public string? ImageName { get; set; }

    public virtual ICollection<RefRequestStamp> RefRequestStamps { get; set; } = new List<RefRequestStamp>();

    public virtual CodeStampStatus StampStatusNavigation { get; set; } = null!;

    public virtual CodeStampType StampTypeNavigation { get; set; } = null!;
}

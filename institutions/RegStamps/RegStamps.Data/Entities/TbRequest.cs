using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbRequest
{
    public int RequestId { get; set; }

    public int SchoolId { get; set; }

    public int RequestType { get; set; }

    public int RequestStatus { get; set; }

    public DateTime RequestDate { get; set; }

    public string? SignerName { get; set; }

    public string? SignerEmail { get; set; }

    public string? Organisation { get; set; }

    public string? SignerBulstat { get; set; }

    public string? SignerIdent { get; set; }

    public string? Signature { get; set; }

    public DateTime? SignTimeStamp { get; set; }

    public virtual ICollection<RefRequestStamp> RefRequestStamps { get; set; } = new List<RefRequestStamp>();

    public virtual CodeRequestStatus RequestStatusNavigation { get; set; } = null!;

    public virtual CodeRequestType RequestTypeNavigation { get; set; } = null!;

    public virtual ICollection<TbRequestFile> TbRequestFiles { get; set; } = new List<TbRequestFile>();
}

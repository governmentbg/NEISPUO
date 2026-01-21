using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class RequestForStampToApprove
{
    public int RequestId { get; set; }

    public int StampId { get; set; }

    public int SchoolId { get; set; }

    public DateTime TimeStamp { get; set; }

    public int StampStatus { get; set; }

    public string StampStatusName { get; set; } = null!;

    public int StampType { get; set; }

    public string StampTypeName { get; set; } = null!;

    public int RequestStatus { get; set; }

    public string RequestStatusName { get; set; } = null!;

    public int RequestType { get; set; }

    public string RequestTypeName { get; set; } = null!;

    public string OblName { get; set; } = null!;

    public string MunicipalityName { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string SchoolName { get; set; } = null!;

    public string SchlMidName { get; set; } = null!;
}

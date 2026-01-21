using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbErrorLog
{
    public int ErrorLogId { get; set; }

    public string? ActionName { get; set; }

    public string? ControllerName { get; set; }

    public string? Url { get; set; }

    public int? SchoolId { get; set; }

    public string? ErrorMessage { get; set; }

    public string? ErrorInnerMessage { get; set; }

    public string? ErrorStackTrace { get; set; }

    public DateTime? TimeStamp { get; set; }
}

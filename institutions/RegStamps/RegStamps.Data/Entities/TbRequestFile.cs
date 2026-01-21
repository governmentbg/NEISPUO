using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class TbRequestFile
{
    public int FileId { get; set; }

    public int RequestId { get; set; }

    public int SchoolId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileData { get; set; } = null!;

    public int FileType { get; set; }

    public DateTime TimeStamp { get; set; }

    public virtual CodeFileType FileTypeNavigation { get; set; } = null!;

    public virtual TbRequest TbRequest { get; set; } = null!;
}

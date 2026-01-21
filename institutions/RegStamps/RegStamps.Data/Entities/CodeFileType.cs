using System;
using System.Collections.Generic;

namespace RegStamps.Data.Entities;

public partial class CodeFileType
{
    public int FileTypeId { get; set; }

    public string FileTypeName { get; set; } = null!;

    public virtual ICollection<TbRequestFile> TbRequestFiles { get; set; } = new List<TbRequestFile>();
}

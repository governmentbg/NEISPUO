namespace MON.Models.StudentModels.Lod
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class LodSignatureUndoModel
    {
        public int PersonId { get; set; }
        public short SchoolYear { get; set; }
        public string Reason { get; set; }
        public int? ClassId { get; set; }
    }
}

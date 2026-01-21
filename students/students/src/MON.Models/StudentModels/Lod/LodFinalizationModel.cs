using System.Collections.Generic;

namespace MON.Models.StudentModels.Lod
{
    public class LodFinalizationModel
    {
        public IEnumerable<int> PersonIds { get; set; }
        public short SchoolYear { get; set; }
        public int? ClassId { get; set; }
    }
}

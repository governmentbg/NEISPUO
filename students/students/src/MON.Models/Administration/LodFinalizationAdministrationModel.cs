namespace MON.Models.Administration
{
    using System.Collections.Generic;

    public class LodFinalizationAdministrationModel
    {
        public short SchoolYear { get; set; }

        public List<int> PersonIds { get; set; } = new List<int>();
    }
}
